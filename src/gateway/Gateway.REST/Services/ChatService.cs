using QuickChat.Gateway.REST.Models;

namespace QuickChat.Gateway.REST.Services;

public class ChatService(Chat.ChatClient client, ICurrentUserProvider currentUserProvider)
    : IChatService
{
    private readonly Chat.ChatClient client = client;
    private readonly ICurrentUserProvider currentUserProvider = currentUserProvider;

    public async Task<ChatModel> GetChatByCode(string code)
    {
        GetChatByCodeRequest request = new() { Code = code };
        GetChatByCodeReply reply = await client.GetChatByCodeAsync(request);
        return MapChat(reply.Chat);
    }

    public async Task<IList<ChatModel>> GetCurrentUserChats()
    {
        GetUserChatsRequest request =
            new() { UserId = currentUserProvider.GetCurrentUserId().ToString() };
        GetUserChatsReply reply = await client.GetUserChatsAsync(request);
        return [.. reply.Chats.Select(MapChat)];
    }

    public async Task<ChatModel> CreateChat(string name)
    {
        CreateChatRequest request = new() { Name = name };
        CreateChatReply reply = await client.CreateChatAsync(request);
        return MapChat(reply.Chat);
    }

    public async Task<ChatParticipantModel> JoinChat(Guid chatId, string name)
    {
        JoinChatRequest request =
            new()
            {
                ChatId = chatId.ToString(),
                Name = name,
                UserId = currentUserProvider.GetCurrentUserId().ToString()
            };
        JoinChatReply reply = await client.JoinChatAsync(request);
        return MapChatParticipant(reply.Participant);
    }

    private static ChatModel MapChat(ChatMessage message)
    {
        return new ChatModel(
            new Guid(message.Id),
            message.Name,
            message.Code,
            [.. message.Participants.Select(MapChatParticipant)],
            message.CreatedAt.ToDateTime(),
            message.LifeTimeSeconds
        );
    }

    private static ChatParticipantModel MapChatParticipant(ChatParticipantMessage message)
    {
        return new ChatParticipantModel(
            new Guid(message.Id),
            new Guid(message.UserId),
            message.Name,
            message.JoinedAt.ToDateTime()
        );
    }
}
