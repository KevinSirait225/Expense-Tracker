using System;

namespace ExpenseTracker.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;

public class PasswordService
{
    private readonly PasswordHasher<object> _hasher = new();

    // Hashing password 
    public string Hash(string password)
        => _hasher.HashPassword(null!,password);
    
    public bool Verify(string hash, string password)
        => _hasher.VerifyHashedPassword(null!, hash, password)
        == PasswordVerificationResult.Success;
}
