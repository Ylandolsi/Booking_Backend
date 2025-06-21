using Application.Abstractions.Email;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Email;

internal sealed class EmailTemplateProvider : IEmailTemplateProvider
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<EmailTemplateProvider> _logger;
    private const string TemplateNamespace = "Infrastructure.Email.Templates";

    public EmailTemplateProvider(IMemoryCache memoryCache, ILogger<EmailTemplateProvider> logger)
    {
        _logger = logger;
        _memoryCache = memoryCache;
    }

    public async Task<(string Subject, string Body)> GetTemplateAsync(
        string templateName,
        CancellationToken cancellationToken = default)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"EmailTemplate_{templateName}",
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);
                return await ReadTemplateFromFileAsync(templateName, cancellationToken);
            });
    }

    private async Task<(string Subject, string Body)> ReadTemplateFromFileAsync(
        string templateName,
        CancellationToken cancellationToken)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourcePath = GetResourcePath(templateName);

        await using var stream = assembly.GetManifestResourceStream(resourcePath);

        if (stream is null)
        {
            _logger.LogCritical("Email template resource '{ResourcePath}' not found in assembly.", resourcePath);
            throw new FileNotFoundException("Email template file not found.", resourcePath);
        }

        using var reader = new StreamReader(stream);

        var subject = await reader.ReadLineAsync(cancellationToken) ?? string.Empty;
        var body = await reader.ReadToEndAsync(cancellationToken);

        return (subject, body);
    }

    private static string GetResourcePath(string templateName)
    {
        return $"{TemplateNamespace}.{templateName}.html";
    }
}