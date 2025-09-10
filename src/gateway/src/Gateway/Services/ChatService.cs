using QuickChat.Gateway.Extensions;
using QuickChat.Gateway.Models;

namespace QuickChat.Gateway.Services;

public class ChatService(Chat.ChatClient client, ICurrentUserProvider currentUserProvider)
    : IChatService
{
    private readonly Chat.ChatClient client = client;
    private readonly ICurrentUserProvider currentUserProvider = currentUserProvider;

    public async Task<ChatModel> GetChatByCode(string code)
    {
        GetChatByCodeRequest request = new() { Code = code };
        GetChatByCodeReply reply = await client.GetChatByCodeAsync(request);
        return reply.Chat.ToModel();
    }

    public async Task<IList<ChatModel>> GetCurrentUserChats()
    {
        GetUserChatsRequest request =
            new() { UserId = currentUserProvider.GetCurrentUserId().ToString() };
        GetUserChatsReply reply = await client.GetUserChatsAsync(request);
        return [.. reply.Chats.Select(c => c.ToModel())];
    }

    public async Task<ChatModel> CreateChat(string name)
    {
        CreateChatRequest request = new() { Name = name };
        CreateChatReply reply = await client.CreateChatAsync(request);
        return reply.Chat.ToModel();
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
        return reply.Participant.ToModel();
    }
}
