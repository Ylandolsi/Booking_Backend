using Domain.ImageProcessor;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions.Uploads;

public interface IS3ImageProcessingService
{
    Task<bool> DeleteImageAsync(string fileName);
    Task<bool> ImageExistsAsync(string fileName, string suffix);
    Task<ImageProcessingResult> ProcessImageAsync(IFormFile file, string fileName);
}