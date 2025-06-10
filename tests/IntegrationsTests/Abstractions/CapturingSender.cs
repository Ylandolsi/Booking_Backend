using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;

namespace IntegrationsTests.Abstractions;


public class CapturingSender : ISender
{
    public List<IFluentEmail> SentEmails { get; } = new List<IFluentEmail>();

    public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
    {
        SentEmails.Add(email);
        return new SendResponse { MessageId = Guid.NewGuid().ToString() };
    }

    public Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
    {
        SentEmails.Add(email);
        return Task.FromResult(new SendResponse { MessageId = Guid.NewGuid().ToString() });
    }

    public void Clear()
    {
        SentEmails.Clear();
    }
}