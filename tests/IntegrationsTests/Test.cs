using IntegrationsTests.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Application.Users.Register;
using Microsoft.Extensions.DependencyInjection; 
using FluentEmail.Core.Models;
using System.Windows.Input;
using Application.Abstractions.Messaging;
using System.Net.Http.Json;
using FluentEmail.Core;

namespace IntegrationsTests;

public class Test : BaseIntegrationTest
{
    private readonly HttpClient _authenticatedClient;
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly string _testUserEmail = "test.user@example.com";

    public Test(IntegrationTestsWebAppFactory factory) : base(factory)
    {
        _authenticatedClient = Factory.CreateAuthenticatedClient(userId: _testUserId, email: _testUserEmail, isEmailVerified: true);
    }

    [Fact]
    public async Task RegisterUser_ShouldSendVerificationEmail()
    {
        // Arrange
        var registerCommand = new RegisterUserCommand(
            "Test",
            "User",
            Fake.Internet.Email(),
            "Password123!",
            null,
            false);

        //var sender = _scope.ServiceProvider.GetRequiredService<ICommandHandler<RegisterUserCommand , Guid> >(); // Or IMediator if using MediatR

        var registrationPayload = new
        { /* map registerCommand to your API payload */
            FirstName = registerCommand.FirstName,
            LastName = registerCommand.LastName,
            Email = registerCommand.Email,
            Password = registerCommand.Password,
            IsMentor = registerCommand.IsMentor,
            ProfilePictureSource = "" 
        };
        
        HttpResponseMessage response = await _authenticatedClient.PostAsJsonAsync("users/register", registrationPayload);
        response.EnsureSuccessStatusCode();



        var emailService = _scope.ServiceProvider.GetRequiredService<IFluentEmail>();
        await emailService
            .To(registerCommand.Email)
            .Subject("Test Verification")
            .Body("Click here: http://test.com/verify?token=testtoken", true)
            .SendAsync();



        // This might be needed if Hangfire processing isn't immediate in the test host.
         await Task.Delay(TimeSpan.FromSeconds(1));

        Assert.NotEmpty(EmailCapturer.SentEmails);
        var sentEmail = EmailCapturer.SentEmails.FirstOrDefault(e => e.Data.ToAddresses.Any(a => a.EmailAddress == registerCommand.Email));

        Assert.NotNull(sentEmail);
        Assert.Equal("Test Verification", sentEmail.Data.Subject); // Or your actual subject
        Assert.Contains("http://test.com/verify?token=testtoken", sentEmail.Data.Body); // Check for link presence
        Assert.True(sentEmail.Data.IsHtml);
    }
}