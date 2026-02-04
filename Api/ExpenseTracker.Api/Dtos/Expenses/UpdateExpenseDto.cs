using System;

namespace ExpenseTracker.Api.Dtos.Expenses;
using System.ComponentModel.DataAnnotations;
public class UpdateExpenseDto
{
    [Required]
    public decimal Amount {get; set;}

    [Required]
    public string Description{get; set;}="";
}
