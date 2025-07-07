using Application.Abstractions.Messaging;
using Application.ImageProcessor.Upload;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.ImageProcessor;

public class Upload : IEndpoint
{
    public record Request(IFormFile File);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ImageProcessorEndpoints.UploadEndpoint, async ([FromForm] Request request,
                                                                   ICommandHandler<UploadCommand, ImageUploadResult> handler) =>
        {
            UploadCommand command = new(request.File);

            var result = await handler.Handle(command, CancellationToken.None);
            return result.Match((result) => Results.Ok(result), (result) => CustomResults.Problem(result));
        })
        .WithName("UploadImage")
        .Produces<ImageUploadResult>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithTags("Image Processor")
        .DisableAntiforgery()
        .RequireAuthorization();
    }
}
