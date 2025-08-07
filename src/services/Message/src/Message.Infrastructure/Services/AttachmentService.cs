using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using QuickChat.Message.Application.Exceptions;
using QuickChat.Message.Application.Services;
using QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.Infrastructure.Services;

public class AttachmentService(IOptions<AttachmentOptions> options, HttpClient client)
    : IAttachmentService
{
    private readonly AttachmentOptions options = options.Value;
    private readonly HttpClient client = client;

    public async Task<IList<MessageAttachment>> GetAttachments(IEnumerable<Guid> ids)
    {
        string requestUrl = GetAttachmentsUrl(ids);
        HttpResponseMessage response = await InvokeGetAttachments(requestUrl);
        return await ParseGetAttachmentsResponse(response);
    }

    private string GetAttachmentsUrl(IEnumerable<Guid> ids)
    {
        Dictionary<string, StringValues> queryParams =
            new() { ["ids"] = new StringValues([.. ids.Select(id => id.ToString())]) };
        return QueryHelpers.AddQueryString(options.Url + "/attachments", queryParams);
    }

    private async Task<HttpResponseMessage> InvokeGetAttachments(string requestUrl)
    {
        HttpResponseMessage response;

        try
        {
            response = await client.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new AttachmentServiceException(
                    $"Request to '{requestUrl}' failed with status code {(int)response.StatusCode} ({response.ReasonPhrase})"
                );
            }
        }
        catch (HttpRequestException e)
        {
            throw new AttachmentServiceException(
                $"An error occurred while sending the HTTP request to '{requestUrl}",
                e
            );
        }

        return response;
    }

    private static async Task<IList<MessageAttachment>> ParseGetAttachmentsResponse(
        HttpResponseMessage response
    )
    {
        List<MessageAttachment> attachments = await response.Content.ReadFromJsonAsync<List<MessageAttachment>>();

        foreach (MessageAttachment attachment in attachments)
        {
            attachment.AttachmentId = attachment.Id;
            attachment.Id = default;
        }

        return attachments;
    }
}
