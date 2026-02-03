using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Services;
using ExpenseTracker.Api.Dtos.Auth;
using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.Dtos;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

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
})
.RequireAuthorization();

app.Run();
