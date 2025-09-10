using Microsoft.EntityFrameworkCore;
using QuickChat.Message.Application.Repositories;
using QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.Infrastructure.Repositories;

public class SystemMessageRepository(MessageContext context) : ISystemMessageRepository
{
    public IUnitOfWork UnitOfWork => context;
    private readonly MessageContext context = context;

    public void Add(SystemMessage message)
    {
        context.SystemMessages.Add(message);
    }

    public async Task<IList<SystemMessage>> GetChatSystemMessagesAsync(
        Guid chatId,
        int? limit,
        long? cursor
    )
    {
        IQueryable<SystemMessage> query = context
            .SystemMessages.Where(m => m.ChatId == chatId)
            .OrderByDescending(e => e.Id);

        if (cursor != null)
        {
            query = query.Where(m => m.Id < cursor.Value);
        }
        if (limit != null)
        {
            query = query.Take(limit.Value);
        }

        return await query.ToListAsync();
    }
}
