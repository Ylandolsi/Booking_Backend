using Application.Abstractions.Messaging;
using Application.Users.Register;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using IntegrationsTests.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xunit;

namespace IntegrationsTests;





public class AuthenticationTest : BaseIntegrationTest
{
    private readonly HttpClient _authenticatedVerifiedUserClient;
    private readonly Guid _verifiedUserId = Guid.NewGuid();
    private readonly string _verifiedUserEmail = "verified.user@example.com";
    private readonly HttpClient _client ;

    public AuthenticationTest(IntegrationTestsWebAppFactory factory) : base(factory)
    {
        
        _authenticatedVerifiedUserClient = Factory.CreateAuthenticatedClient(userId: _verifiedUserId, email: _verifiedUserEmail, isEmailVerified: true);

        _client = Factory.CreateClient(); 
    }

    [Fact]
    public async Task RegisterUser_ShouldSendVerificationEmail_AndReturnUserId()
    {
        EmailCapturer.Clear(); 


        var registrationPayload = new
        {
            FirstName = "Test",
            LastName = "UserReg",
            Email = Fake.Internet.Email(),
            Password = "Password123!",
            IsMentor = true ,
            ProfilePictureSource = "" 
        };

       
        HttpResponseMessage response = await _client.PostAsJsonAsync("users/register", registrationPayload);
        response.EnsureSuccessStatusCode(); 

        
        var registrationResponse = await response.Content.ReadFromJsonAsync<Guid>();

        Assert.NotNull(registrationResponse);
        Assert.NotEqual(Guid.Empty, registrationResponse);

        
        await Task.Delay(TimeSpan.FromSeconds(2)); 

        Assert.NotEmpty(EmailCapturer.SentEmails);
        var sentEmail = EmailCapturer.SentEmails.FirstOrDefault(e => e.Data.ToAddresses.Any(a => a.EmailAddress == registrationPayload.Email));

        Assert.NotNull(sentEmail);
        
        Assert.Contains("click here", sentEmail.Data.Body, StringComparison.OrdinalIgnoreCase);
        Assert.True(sentEmail.Data.IsHtml);
    }



    [Fact]
    public async Task ReSendVerificationEmail_ShouldSendNewVerificationEmail_WhenUserNotVerified()
    {
        EmailCapturer.Clear();

       
        var newUserEmail = Fake.Internet.Email();
        var newUserPassword = "Password123!";
        var registerPayload = new
        {
            FirstName = "Unverified",
            LastName = "User",
            Email = newUserEmail,
            Password = newUserPassword,
            IsMentor = false,
            ProfilePictureSource = ""
        };

        HttpResponseMessage registerResponse = await _client.PostAsJsonAsync("users/register", registerPayload);
        registerResponse.EnsureSuccessStatusCode();
        var newUserId = await registerResponse.Content.ReadFromJsonAsync<Guid>();

        EmailCapturer.Clear();

        
        HttpClient unverifiedUserClient = Factory.CreateAuthenticatedClient(userId: newUserId, email: newUserEmail, isEmailVerified: false);

       
        HttpResponseMessage resendResponse = await unverifiedUserClient.PostAsync("users/resend-verification-email", null);
        resendResponse.EnsureSuccessStatusCode(); 

        
        await Task.Delay(TimeSpan.FromSeconds(2));

        Assert.NotEmpty(EmailCapturer.SentEmails);
        
        var sentEmail = EmailCapturer.SentEmails.FirstOrDefault(e => e.Data.ToAddresses.Any(a => a.EmailAddress == newUserEmail));

        Assert.NotNull(sentEmail);
        Assert.Contains("click here", sentEmail.Data.Body, StringComparison.OrdinalIgnoreCase);
        Assert.True(sentEmail.Data.IsHtml);
    }

    [Fact]
    public async Task ReSendVerificationEmail_ShouldReturnError_WhenUserAlreadyVerified()
    {
        EmailCapturer.Clear();

        
        HttpResponseMessage response = await _authenticatedVerifiedUserClient.PostAsync("users/resend-verification-email", null);


        Assert.False(response.IsSuccessStatusCode);


    }
}


