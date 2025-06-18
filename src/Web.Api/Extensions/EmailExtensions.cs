namespace Web.Api.Extensions;

public static class EmailExtensions
{
    public static IServiceCollection AddEmailSender(this IServiceCollection services, IConfiguration configuration)
    {
        // var host = configuration?["Email:Host"] ?? throw new ArgumentException("Email Host is not configured ");
        // var port = configuration?.GetValue<int>("Email:Port") ?? throw new ArgumentException("Email Port is not configured ");
        // var senderEmail = configuration?["Email:SenderEmail"] ?? throw new ArgumentException("Email SenderEmail is not configured ");
        // var senderName = configuration?["Email:Sender"] ?? throw new ArgumentException("Email Sender is not configured ");

        // services.AddFluentEmail(senderEmail, senderName)
        //     .AddSmtpSender(host, port);

        return services;
    }
}