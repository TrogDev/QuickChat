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
            .Select(
                c =>
                    new
                    {
                        UserInChat = c.Participants.FirstOrDefault(p => p.UserId == userId),
                        Chat = c
                    }
            )
            .Where(x => x.UserInChat != null)
            .OrderByDescending(c => c.UserInChat.JoinedAt)
            .Select(x => x.Chat)
            .ToListAsync();
    }

    private static IQueryable<Domain.Entities.Chat> ExcludeExpired(
        IQueryable<Domain.Entities.Chat> query
    )
    {
        return query.Where(c => c.CreatedAt.AddSeconds(c.LifeTimeSeconds) >= DateTime.UtcNow);
    }
}
