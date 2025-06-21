namespace Application.Abstractions.Email;

public interface IEmailTemplateProvider
{
    Task<(string Subject, string Body)> GetTemplateAsync(
        string templateName,
        CancellationToken cancellationToken = default);
}