using Microsoft.EntityFrameworkCore;
using QuickChat.Attachment.Application.Repositories;

namespace QuickChat.Attachment.Infrastructure;

public class AttachmentContext : DbContext, IUnitOfWork
{
    public DbSet<Domain.Entities.Attachment> Attachments { get; set; }

    public AttachmentContext(DbContextOptions<AttachmentContext> options)
        : base(options) { }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await base.SaveChangesAsync(cancellationToken);
        return true;
    }
}
