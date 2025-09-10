using Microsoft.AspNetCore.SignalR;

namespace QuickChat.Gateway.Hubs;

public class ChatHub : Hub<IChatHubClient>
{
    public async Task SubscribeChat(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task UnsubscribeChat(string chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
    }
}
