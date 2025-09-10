using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.Services;

public class SystemMessageService(SystemMessage.SystemMessageClient client) : ISystemMessageService
{
    private readonly SystemMessage.SystemMessageClient client = client;

    public async Task<IList<SystemMessageModel>> GetChatSystemMessages(
        Guid chatId,
        int limit,
        long? cursor
    )
    {
        GetChatSystemMessagesRequest request = new() { ChatId = chatId.ToString(), Limit = limit };

        if (cursor != null)
        {
            request.Cursor = cursor.Value;
        }

        GetChatSystemMessagesReply reply = await client.GetChatSystemMessagesAsync(request);
        return [.. reply.Messages.Select(MapSystemMessage)];
    }

    private static SystemMessageModel MapSystemMessage(SystemMessageMessage message)
    {
        return new SystemMessageModel(
            message.Id,
            new Guid(message.ChatId),
            message.Text,
            message.CreatedAt.ToDateTime()
        );
    }
}
