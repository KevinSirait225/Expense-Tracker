using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Services;
using ExpenseTracker.Api.Dtos.Auth;
using ExpenseTracker.Api.Dtos.Expenses;
using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.Dtos;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

// Ambil user id
int GetUserId(ClaimsPrincipal user)
{
    return int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<JwtService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "API running");

// AUTH ROUTES
// Register akun
app.MapPost("/auth/register", async (
    RegisterDto dto,
    AppDbContext db,
    PasswordService passwordService
) =>
{
    // Email harus unik
    var exist = await db.Users.AnyAsync(u => u.Email == dto.Email);
    // Jika sudah ada tampilkan message
    if(exist) return Results.BadRequest(new {message = "Email already registered"});

    // User baru
    var user = new User
    {
        Email= dto.Email,
        PasswordHash = passwordService.Hash(dto.Password) // password di hash dulu dgn service, lalu disimpan
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Created("/auth/register", new
    {
        user.Id,
        user.Email
    });
});

// Login akun yang ada
app.MapPost("/auth/login", async (
    LoginDto dto,
    AppDbContext db,
    PasswordService passwordService,
    JwtService jwtService
) =>
{
    // cari user email di db, kalau tdk ada message badrequest
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
    if (user == null) return Results.BadRequest(new {message  = "Invalid credentials"});
    
    // verify password yg di input sesuai dgn yg hash yg disimpan atau tdk
    var valid = passwordService.Verify(user.PasswordHash, dto.Password);
    if (!valid) return Results.BadRequest(new {message  = "Invalid credentials"});

    var token = jwtService.Generate(user);

    return Results.Ok(new
    {
        token
    });
});

// EXPENSES ROUTES
app.MapPost("/expenses", async (
    CreateExpenseDto dto,
    AppDbContext db,
    ClaimsPrincipal user
) =>
{
    // Ambl user id dari user yang sedang login
    var userId = GetUserId(user);
    var expense = new Expense
    {
        Amount = dto.Amount,
        Description = dto.Description,
        UserId = userId
    };

    db.Expenses.Add(expense);
    await db.SaveChangesAsync();

    // Response untuk kontrol apa aja yang keluar
    var response = new ExpenseDto
    {
        Id = expense.Id,
        Amount = expense.Amount,
        Description = expense.Description,
        Date = expense.Date
    };

    return Results.Created($"/expenses/{expense.Id}", response);

}).RequireAuthorization();

app.MapGet("/expenses", async (
    AppDbContext db,
    ClaimsPrincipal user
) =>
{
    var userId = GetUserId(user);
    var expenses = await db.Expenses
        .Where(e => e.UserId == userId)
        .OrderByDescending(e => e.Date)
        .Select( e=> new ExpenseDto
        {
            Id = e.Id,
            Amount = e.Amount,
            Description = e.Description,
            Date = e.Date
        })
        .ToListAsync();
    return Results.Ok(expenses);
}).RequireAuthorization();

app.MapPut("/expenses/{id:int}", async (
    int id, UpdateExpenseDto dto, AppDbContext db, ClaimsPrincipal user
) =>
{
    var userId = GetUserId(user);
    var expense= await db.Expenses
        .FirstOrDefaultAsync(e=>e.Id == id && e.UserId == userId);

    if(expense==null) return Results.NotFound();

    expense.Amount = dto.Amount;
    expense.Description = dto.Description;

    await db.SaveChangesAsync();
    return Results.NoContent();

}).RequireAuthorization();

app.MapDelete("/expenses/{id:int}", async (
    int id, AppDbContext db, ClaimsPrincipal user
) =>
{
    var userId = GetUserId(user);
    var expense= await db.Expenses
        .FirstOrDefaultAsync(e=>e.Id == id && e.UserId == userId);

    if(expense==null) return Results.NotFound();

    db.Expenses.Remove(expense);
    await db.SaveChangesAsync();
    return Results.NoContent();

}).RequireAuthorization();

// Test authorization
app.MapGet("/me", (ClaimsPrincipal user) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    var email = user.FindFirstValue(ClaimTypes.Email);

    return Results.Ok(new
    {
        userId,
        email
    });
}).RequireAuthorization();

app.Run();
