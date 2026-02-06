using System;

namespace ExpenseTracker.Api.Errors;

public class UnauthorizedException : ApiException
{
    public override int StatusCode => 401;
    public override string ErrorType => "Unauthorized";
    public UnauthorizedException(string message): base(message){}
}
