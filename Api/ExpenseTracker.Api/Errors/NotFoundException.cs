using System;

namespace ExpenseTracker.Api.Errors;

public class NotFoundException : ApiException
{
    public override int StatusCode => 404;
    public override string ErrorType => "NotFound";
    public NotFoundException(string message): base(message){}
}
