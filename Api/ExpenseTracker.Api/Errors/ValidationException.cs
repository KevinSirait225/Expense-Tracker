using System;

namespace ExpenseTracker.Api.Errors;

public class ValidationException : ApiException
{
    public override int StatusCode => 400;
    public override string ErrorType => "ValidationError";
    public ValidationException(string message): base(message){}
}
