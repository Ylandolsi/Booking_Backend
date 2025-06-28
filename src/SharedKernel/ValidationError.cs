namespace SharedKernel;

public sealed record ValidationError : Error
{
    public ValidationError(Error[] errors)
        : base(
            "Validation.General",
            "One or more validation errors occurred",
            ErrorType.Validation)
    {
        Errors = errors;
    }

    public Error[] Errors { get; }

    public ValidationError ( Error error ) 
        : this([ error ])
    {
    }
    public ValidationError(string code, string description ) 
        : this([ Error.Problem(code, description )  ])
    {
    }

    public static ValidationError FromResults(IEnumerable<Result> results) =>
        new(results.Where(r => r.IsFailure).Select(r => r.Error).ToArray());
}

/* 

var errors = new List<ValidationError>();

if (string.IsNullOrEmpty(email))
    errors.Add(new ValidationError("Email", "Email is required"));

if (string.IsNullOrEmpty(name))
    errors.Add(new ValidationError("Name", "Name is required"));

if (errors.Any())
{
    return Result.Failure<User>(
        Error.Validation("User.Validation", "User validation failed", errors)
    );




{
"status":400,"detail":"The provided email or password is incorrect. Please try again.","traceId":"00-418f39a963aa53e4393b907e8542df06-2f18b0edea9851f8-00"}

    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "User.Validation",
    "status": 400,
    "detail": "User validation failed",
    "errors": [
        {
            "Code": "Email",
            "errorMessage": "Email is required"
        },
        {
            "Code": "Name", 
            "errorMessage": "Name is required"
        }
    ]
}
*/ 