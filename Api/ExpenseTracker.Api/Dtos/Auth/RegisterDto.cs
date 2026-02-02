using System;

namespace ExpenseTracker.Api.Dtos.Auth;

using System.ComponentModel.DataAnnotations;

// DTO register user, biar data yang masuk sesuai
public class RegisterDto
{
    [Required, EmailAddress]
    public string Email {get; set;} = "";

    [Required, MinLength(6)]
    public string Password{get; set;}= "";
}
