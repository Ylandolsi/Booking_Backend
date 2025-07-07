using Application.Abstractions.Uploads;
using Domain.ImageProcessor;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Uploads;

public class LocalFileImageProcessingService : IS3ImageProcessingService
{
    private readonly ILogger<LocalFileImageProcessingService> _logger;
    private readonly string _baseStoragePath;
    private readonly string _baseUrl;

    public LocalFileImageProcessingService(ILogger<LocalFileImageProcessingService> logger, IConfiguration config)
    {
        _logger = logger;
        _baseStoragePath = config["LocalStorage:BasePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        _baseUrl = config["LocalStorage:BaseUrl"] ?? "http://localhost:5000/uploads";

        // Ensure directory exists
        Directory.CreateDirectory(_baseStoragePath);
        Directory.CreateDirectory(Path.Combine(_baseStoragePath, "images"));
    }

    public async Task<ImageProcessingResult> ProcessImageAsync(IFormFile file, string fileName)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is null or empty");

        try
        {
            using var image = await Image.LoadAsync(file.OpenReadStream());

            if (image.Width > 10000 || image.Height > 10000)
                throw new ArgumentException("Image dimensions too large");

            var result = new ImageProcessingResult
            {
                OriginalUrl = await SaveImageToFileSystemAsync(image, fileName, "original", 75),
                ThumbnailUrl = await SaveThumbnailToFileSystemAsync(image, fileName, 50, 5)
            };

            return result;
        }
        catch (UnknownImageFormatException ex)
        {
            _logger.LogError(ex, "Invalid image format for file {FileName}", fileName);
            throw new ArgumentException("Invalid image format");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing image {FileName}", fileName);
            throw;
        }
    }

    private async Task<string> SaveImageToFileSystemAsync(Image image, string fileName, string suffix, int quality)
    {
        var relativePath = $"images/{fileName}_{suffix}.jpg";
        var fullPath = Path.Combine(_baseStoragePath, relativePath);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await image.SaveAsync(stream, new JpegEncoder { Quality = quality });

        return $"{_baseUrl}/{relativePath.Replace('\\', '/')}";
    }

    private async Task<string> SaveThumbnailToFileSystemAsync(Image image, string fileName, int size, int quality)
    {
        using var clone = image.CloneAs<SixLabors.ImageSharp.PixelFormats.Rgba32>();

        clone.Mutate(x => x
            .Resize(new ResizeOptions
            {
                Size = new Size(size, size),
                Mode = ResizeMode.Max,
                Sampler = KnownResamplers.Bicubic
            })
            .GaussianBlur(0.5f));

        var relativePath = $"images/{fileName}_thumb.jpg";
        var fullPath = Path.Combine(_baseStoragePath, relativePath);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await clone.SaveAsync(stream, new JpegEncoder { Quality = quality });

        return $"{_baseUrl}/{relativePath.Replace('\\', '/')}";
    }

    public async Task<bool> DeleteImageAsync(string fileName)
    {
        try
        {
            var paths = new[]
            {
                Path.Combine(_baseStoragePath, $"images/{fileName}_original.jpg"),
                Path.Combine(_baseStoragePath, $"images/{fileName}_thumb.jpg")
            };

            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting images for {FileName}", fileName);
            return false;
        }
    }

    public Task<bool> ImageExistsAsync(string fileName, string suffix)
    {
        var path = Path.Combine(_baseStoragePath, $"images/{fileName}_{suffix}.jpg");
        return Task.FromResult(File.Exists(path));
    }
}