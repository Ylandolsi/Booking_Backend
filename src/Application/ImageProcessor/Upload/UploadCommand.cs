using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Http;
using SharedKernel;

namespace Application.ImageProcessor.Upload;

public record UploadCommand(IFormFile File) : ICommand<ImageUploadResult>;
