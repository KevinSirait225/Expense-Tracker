using System;

namespace ExpenseTracker.Api.Models;

public class ErrorResponse
{
    public string Type{get; set;}= "";

    public string Message {get; set;} = "";
}
