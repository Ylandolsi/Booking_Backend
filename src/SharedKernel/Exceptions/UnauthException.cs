namespace SharedKernel.Exceptions;

public class UnauthException : BaseException
{
    public UnauthException(string message = "User is Unauthorized", int code = 401) : base(message, code) { }
}