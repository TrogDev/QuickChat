using Microsoft.EntityFrameworkCore;
using QuickChat.Message.Application.Exceptions;
using QuickChat.Message.Application.Repositories;

namespace QuickChat.Message.Infrastructure.Repositories;

public class MessageRepository(MessageContext context) : IMessageRepository
{
    public IUnitOfWork UnitOfWork => context;
    private readonly MessageContext context = context;

    public void Add(Domain.Entities.Message message)
    {
        context.Messages.Add(message);
    }

    public async Task<Domain.Entities.Message> FindByIdAsync(Guid chatId, long id)
    {
        return await ExcludeDeleted(context.Messages)
                .Include(m => m.Attachments)
                .FirstOrDefaultAsync(m => m.Id == id && m.ChatId == chatId)
            ?? throw new EntityNotFoundException();
    }

    public async Task<IList<Domain.Entities.Message>> GetChatMessagesAsync(
        Guid chatId,
        int? limit,
        long? cursor
    )
    {
        IQueryable<Domain.Entities.Message> query = ExcludeDeleted(context.Messages)
            .Include(m => m.Attachments)
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(e => e.Id);

        if (limit != null)
        {
            query = query.Take(limit.Value);
        }
        if (cursor != null)
        {
            query = query.Where(e => e.Id < cursor.Value);
        }

        return await query.ToListAsync();
    }

    private static IQueryable<Domain.Entities.Message> ExcludeDeleted(
        IQueryable<Domain.Entities.Message> query
    )
    {
        return query.Where(c => !c.IsDeleted);
    }
}
