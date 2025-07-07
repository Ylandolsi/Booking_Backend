using Domain.ImageProcessor;

namespace Application.ImageProcessor.Upload;

public class ImageUploadResult 
{
    public required string Id { get; set; }
    public ImageProcessingResult Urls { get; set; }
}
