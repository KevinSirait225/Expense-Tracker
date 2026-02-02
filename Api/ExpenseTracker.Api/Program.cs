using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Services;
using ExpenseTracker.Api.Dtos.Auth;
using ExpenseTracker.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddScoped<PasswordService>();

var app = builder.Build();

app.MapGet("/", () => "API running");

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

app.Run();
