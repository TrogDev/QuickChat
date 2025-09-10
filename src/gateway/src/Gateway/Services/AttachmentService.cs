using System.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using QuickChat.Gateway.Models;
using QuickChat.Gateway.Options;

namespace QuickChat.Gateway.Services;

public class AttachmentService(
    IServiceHttpClient client,
    IOptions<AttachmentOptions> options,
    ILogger<AttachmentService> logger
) : IAttachmentService
{
    private readonly IServiceHttpClient client = client;
    private readonly ILogger<AttachmentService> logger = logger;
    private readonly AttachmentOptions options = options.Value;

    public async Task<IList<AttachmentModel>> GetAttachments(IEnumerable<Guid> ids)
    {
        logger.LogInformation("Getting attachments with ids {ids}", string.Join(",", ids));

        HttpRequestMessage request = new(HttpMethod.Get, GetAttachmentsUrl(ids));

        return await client.InvokeRequest<List<AttachmentModel>>(request);
    }

    private string GetAttachmentsUrl(IEnumerable<Guid> ids)
    {
        Dictionary<string, StringValues> queryParams =
            new() { ["ids"] = new StringValues([.. ids.Select(id => id.ToString())]) };
        return QueryHelpers.AddQueryString(options.Url + "/attachments", queryParams);
    }

    public async Task<AttachmentModel> UploadAttachment(IFormFile file, Enums.AttachmentType type)
    {
        logger.LogInformation("Uploading an attachment with type {type}", type);

        MultipartFormDataContent content = GetUploadAttachmentContent(
            file,
            type,
            out Stream stream
        );
        HttpRequestMessage request =
            new(HttpMethod.Post, options.Url + "/attachments") { Content = content };
        AttachmentModel response = await client.InvokeRequest<AttachmentModel>(request);
        stream.Dispose();

        return response;
    }

    private static MultipartFormDataContent GetUploadAttachmentContent(
        IFormFile file,
        Enums.AttachmentType attachmentType,
        out Stream stream
    )
    {
        MultipartFormDataContent content = [];

        stream = file.OpenReadStream();
        StreamContent streamContent = new(stream);
        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);

        content.Add(new StreamContent(stream), "file", file.FileName);
        content.Add(new StringContent(Enum.GetName(attachmentType)!), "type");

        return content;
    }
}
