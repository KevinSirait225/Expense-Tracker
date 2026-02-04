using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Api.Models;

public class Expense
{
    public int Id {get; set;}

    [Required]
    public decimal Amount {get; set;}

    [Required]
    public string Description {get; set;} = "";

    public DateTime Date {get; set;} = DateTime.UtcNow;

    // Relationship ke user tertentu
    public int UserId {get; set;}
    public User User {get; set;} = null!;
}
