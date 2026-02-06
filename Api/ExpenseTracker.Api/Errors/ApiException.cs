using System;

namespace ExpenseTracker.Api.Errors;

public abstract class ApiException : Exception
{
    public abstract int StatusCode{get;}
    public abstract string ErrorType{get;}
    protected ApiException(string message): base(message){}
}
