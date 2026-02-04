using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Api.Dtos.Expenses;

public class CreateExpenseDto
{
    [Required]
    public decimal Amount{get; set;}

    [Required]
    public string Description{get; set;}="";
}
