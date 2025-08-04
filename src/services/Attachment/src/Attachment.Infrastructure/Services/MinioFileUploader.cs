using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using QuickChat.Attachment.Application.DTO;
using QuickChat.Attachment.Application.Exceptions;
using QuickChat.Attachment.Application.Services;

namespace QuickChat.Attachment.Infrastructure.Services;

public class MinioFileUploader(IOptions<MinioOptions> options, IAmazonS3 s3Client) : IFileUploader
{
    private readonly MinioOptions options = options.Value;
    private readonly IAmazonS3 s3Client = s3Client;

    public async Task<UploadedFileInfo> Upload(Application.DTO.FileInfo file)
    {
        string extension = Path.GetExtension(file.Name);
        string key = Guid.NewGuid() + extension;

        PutObjectRequest putRequest =
            new()
            {
                BucketName = options.BucketName,
                Key = key,
                InputStream = file.Stream,
                ContentType = file.ContentType
            };

        PutObjectResponse response;

        try
        {
            response = await s3Client.PutObjectAsync(putRequest);
        }
        catch (AmazonS3Exception e)
        {
            throw new FileUploadException("Error occurs during upload", e);
        }
        catch (HttpRequestException e)
        {
            throw new FileUploadException("Error occurs during upload", e);
        }

        return new UploadedFileInfo($"{options.Url}/{options.BucketName}/{key}");
    }
}
