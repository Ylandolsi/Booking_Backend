using Amazon.S3;
using Amazon.S3.Model;
using Application.Abstractions.Uploads;
using Domain.ImageProcessor;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Uploads;

public class S3ImageProcessingService : IS3ImageProcessingService
{
    // TODO : add a CDN option for public images, e.g., CloudFront or similar
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<S3ImageProcessingService> _logger;
    private readonly string _bucketName;
    //private readonly string _cloudFrontUrl;

    public S3ImageProcessingService(IAmazonS3 s3Client, ILogger<S3ImageProcessingService> logger, IConfiguration config)
    {
        _s3Client = s3Client;
        _logger = logger;
        _bucketName = config["AWS:S3:BucketName"] ?? throw new ArgumentNullException("AWS:S3:BucketName configuration is missing");
        //_cloudFrontUrl = config["AWS:CloudFront:Url"]; // Optional: for CDN
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
                // original is treated as the meduim size for now
                OriginalUrl = await UploadImageToS3Async(image, fileName, "original", 75),
                ThumbnailUrl = await UploadThumbnailToS3Async(image, fileName, 50, 5) // blurry for lazy loading
                // TODO : for future use, if needed
                //LargeUrl = await UploadResizedImageToS3Async(image, fileName, "large", 1200, 80),
                //MediumUrl = await UploadResizedImageToS3Async(image, fileName, "medium", 800, 80),
                //SmallUrl = await UploadResizedImageToS3Async(image, fileName, "small", 400, 75),
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

    private async Task<string> UploadImageToS3Async(Image image, string fileName, string suffix, int quality)
    {
        var key = $"images/{fileName}_{suffix}.jpg";

        using var stream = new MemoryStream();
        await image.SaveAsync(stream, new JpegEncoder { Quality = quality });
        stream.Position = 0;

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = stream,
            ContentType = "image/jpeg",
            // u should enable cannelACL first in AWS setting 
            // CannedACL = S3CannedACL.Private,
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
            Metadata =
            {
                ["original-filename"] = fileName,
                ["processed-date"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                ["image-type"] = suffix
            }
        };

        await _s3Client.PutObjectAsync(request);
        return GetSignedUrl(key);
    }



    private async Task<string> UploadThumbnailToS3Async(Image image, string fileName, int size, int quality)
    {
        using var clone = image.CloneAs<SixLabors.ImageSharp.PixelFormats.Rgba32>();

        clone.Mutate(x => x
            .Resize(new ResizeOptions
            {
                Size = new Size(size, size),
                Mode = ResizeMode.Max, // keep the aspect ratio
                Sampler = KnownResamplers.Bicubic // a fast resampling algorithm
            })
            .GaussianBlur(0.5f)); // Add blur for lazy loading

        var key = $"images/{fileName}_thumb.jpg";

        using var stream = new MemoryStream();
        await clone.SaveAsync(stream, new JpegEncoder { Quality = quality });
        stream.Position = 0;

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = stream,
            ContentType = "image/jpeg",
            // u should enable cannelACL first in AWS setting 
            // TODO : makes the object private when we use a CDN, otherwise it will be public
            //CannedACL = S3CannedACL.Private, // REQUIRES SIGNED URL FOR PRIVATE OBJECTS
            // CannedACL = S3CannedACL.PublicRead, // For public access if not using a CDN
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
            Metadata =
            {
                ["original-filename"] = fileName,
                ["processed-date"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                ["image-type"] = "thumbnail",
                ["size"] = size.ToString()
            }
        };

        await _s3Client.PutObjectAsync(request);
        return GetSignedUrl(key);
    }

    private string GetSignedUrl(string key)
    {
        // Generate signed URL for private objects (expires in 24 hours)
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key,
            Verb = HttpVerb.GET,
            // TODO : set expiration time when we use a CDN
            Expires = DateTime.UtcNow.AddHours(24)
            //Expires = DateTime.UtcNow.AddYears(DateTime.MaxValue.Year - DateTime.UtcNow.Year)
        };

        return _s3Client.GetPreSignedURL(request);
    }

    // Alternative method for public URLs if using CloudFront
    //private string GetPublicUrl(string key)
    //{
    //    return !string.IsNullOrEmpty(_cloudFrontUrl)
    //        ? $"{_cloudFrontUrl.TrimEnd('/')}/{key}"
    //        : $"https://{_bucketName}.s3.amazonaws.com/{key}";
    //}

    public async Task<bool> DeleteImageAsync(string fileName)
    {
        try
        {
            var keys = new[]
            {
                $"images/{fileName}_original.jpg",
                $"images/{fileName}_thumb.jpg"
                //$"images/{fileName}_large.jpg",
                //$"images/{fileName}_medium.jpg",
                //$"images/{fileName}_small.jpg",
            };

            var deleteRequest = new DeleteObjectsRequest
            {
                BucketName = _bucketName,
                Objects = keys.Select(key => new KeyVersion { Key = key }).ToList()
            };

            var response = await _s3Client.DeleteObjectsAsync(deleteRequest);

            if (response.DeleteErrors?.Count > 0)
            {
                foreach (var error in response.DeleteErrors)
                {
                    _logger.LogWarning("Failed to delete object {Key}: {Code} - {Message}",
                        error.Key, error.Code, error.Message);
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

    public async Task<bool> ImageExistsAsync(string fileName, string suffix)
    {
        try
        {
            var key = $"images/{fileName}_{suffix}.jpg";
            var request = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if image exists: {FileName}_{Suffix}", fileName, suffix);
            return false;
        }
    }

    // FOR THE DIFFERENT SIZES
    private async Task<string> UploadResizedImageToS3Async(Image image, string fileName, string suffix, int maxSize, int quality)
    {
        using var clone = image.CloneAs<SixLabors.ImageSharp.PixelFormats.Rgba32>();

        if (image.Width > maxSize || image.Height > maxSize)
        {
            clone.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(maxSize, maxSize),
                Mode = ResizeMode.Max, // keep aspect ratio
                Sampler = KnownResamplers.Lanczos3
            }));
        }

        var key = $"images/{fileName}_{suffix}.jpg";

        using var stream = new MemoryStream();
        await clone.SaveAsync(stream, new JpegEncoder { Quality = quality });
        stream.Position = 0;

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = stream,
            ContentType = "image/jpeg",
            // u should enable cannelACL first in AWS setting 

            // TODO : makes the object private when we use a CDN, otherwise it will be public
            //CannedACL = S3CannedACL.Private, // REQUIRES SIGNED URL FOR PRIVATE OBJECTS
            // CannedACL = S3CannedACL.PublicRead, // For public access if not using a CDN
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
            Metadata =
            {
                ["original-filename"] = fileName,
                ["processed-date"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                ["image-type"] = suffix,
                ["max-size"] = maxSize.ToString()
            }
        };

        await _s3Client.PutObjectAsync(request);
        return GetSignedUrl(key);
    }
}

