using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Moq;

namespace IntegrationsTests.Abstractions;

public static class CaptureAmazonSESServiceMock
{
    public static IAmazonSimpleEmailService CreateMock(out List<SendEmailRequest> capturedEmails)
    {
        var localCapturedEmails = new List<SendEmailRequest>();
        var mock = new Mock<IAmazonSimpleEmailService>();

        mock.Setup(svc => svc.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()))
            .Callback<SendEmailRequest, CancellationToken>((req, ct) => localCapturedEmails.Add(req))
            .ReturnsAsync(new SendEmailResponse
            {
                MessageId = Guid.NewGuid().ToString(),
                ResponseMetadata = new Amazon.Runtime.ResponseMetadata
                {
                    ChecksumValidationStatus = Amazon.Runtime.ChecksumValidationStatus.SUCCESSFUL
                }
            });


        capturedEmails = localCapturedEmails;
        return mock.Object;
    }
}