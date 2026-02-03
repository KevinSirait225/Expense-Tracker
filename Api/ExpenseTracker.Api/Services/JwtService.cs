using System;

namespace ExpenseTracker.Api.Services;

using System.IdentityModel.Tokens.Jwt; // Buat create JWT, read/write token jwt
using System.Security.Claims; // utk idenfify user
using System.Text;
using Microsoft.Extensions.Configuration; // biar bisa ambil jwt key di appsettings
using Microsoft.IdentityModel.Tokens; // utk signing token (disini creds dan key)
using ExpenseTracker.Api.Models;

public class JwtService
{
    private readonly IConfiguration _config; // ambil appconfig sebagai read only

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    // Generate token, kalau berhasl login
    public string Generate(User user)
    {
        // info dari user tersebut dimasukkan ke token. disini userID dan email
        var claims=new[]
        {
          new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
          new Claim(ClaimTypes.Email, user.Email)  
        };

        // buat secret key dgn config yg didapat dari appconfig
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        // protect token dgn key yg dibuat tadi dan standard algorithm
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // write tokennya
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"], //ambil dari appconfig
            audience: _config["Jwt:Audience"], //ambil dari appconfig
            claims: claims, // claims yg dibuat diatas
            expires: DateTime.UtcNow.AddHours(1), // token akan expire stlh 1 jam
            signingCredentials: creds // security yg dibuat diatas
        );

        // convert token jadi string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
