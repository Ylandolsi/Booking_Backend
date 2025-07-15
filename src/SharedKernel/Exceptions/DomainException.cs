namespace SharedKernel.Exceptions;

public class DomainException : BaseException
{
    public DomainException(string message = "Domain Exception", int code = 500) : base(message, code) { }
}