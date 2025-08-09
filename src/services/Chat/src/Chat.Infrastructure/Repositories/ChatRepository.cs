using Microsoft.EntityFrameworkCore;
using QuickChat.Chat.Application.Exceptions;
using QuickChat.Chat.Application.Repositories;

namespace QuickChat.Chat.Infrastructure.Repositories;

public class ChatRepository(ChatContext context) : IChatRepository
{
    private readonly ChatContext context = context;
    public IUnitOfWork UnitOfWork => context;

    public void Add(Domain.Entities.Chat chat)
    {
        context.Chats.Add(chat);
    }

    public async Task<Domain.Entities.Chat> GetById(Guid id)
    {
        return await ExcludeExpired(context.Chats)
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == id) ?? throw new EntityNotFoundException();
    }

    public async Task<Domain.Entities.Chat> GetByCode(string code)
    {
        return await ExcludeExpired(context.Chats)
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Code == code) ?? throw new EntityNotFoundException();
    }

    public async Task<IList<Domain.Entities.Chat>> GetByUserId(Guid userId)
    {
        return await ExcludeExpired(context.Chats)
            .Include(c => c.Participants)
            .Where(c => c.Participants.Any(p => p.UserId == userId))
            .ToListAsync();
    }

    private static IQueryable<Domain.Entities.Chat> ExcludeExpired(
        IQueryable<Domain.Entities.Chat> query
    )
    {
        return query.Where(c => c.CreatedAt.AddSeconds(c.LifeTimeSeconds) >= DateTime.UtcNow);
    }
}
