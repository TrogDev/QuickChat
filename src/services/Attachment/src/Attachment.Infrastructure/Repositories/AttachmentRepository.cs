using Microsoft.EntityFrameworkCore;
using QuickChat.Attachment.Application.Exceptions;
using QuickChat.Attachment.Application.Repositories;

namespace QuickChat.Attachment.Infrastructure.Repositories;

public class AttachmentRepository(AttachmentContext context) : IAttachmentRepository
{
    private readonly AttachmentContext context = context;
    public IUnitOfWork UnitOfWork => context;

    public void Add(Domain.Entities.Attachment attachment)
    {
        context.Attachments.Add(attachment);
    }

    public async Task<Domain.Entities.Attachment> FindByIdAsync(Guid id)
    {
        return await context.Attachments.FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new EntityNotFoundException();
    }

    public async Task<IList<Domain.Entities.Attachment>> FindByIdAsync(IEnumerable<Guid> ids)
    {
        return await context.Attachments.Where(e => ids.Contains(e.Id)).ToListAsync();
    }
}
