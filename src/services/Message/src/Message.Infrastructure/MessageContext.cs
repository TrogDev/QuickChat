using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using QuickChat.Message.Application.Repositories;
using QuickChat.Message.Domain.Entities;
using QuickChat.Message.Infrastructure.Configurations;

namespace QuickChat.Message.Infrastructure;

public class MessageContext : DbContext, IUnitOfWork
{
    private readonly IMediator? mediator;

    public DbSet<Domain.Entities.Message> Messages { get; set; }
    public DbSet<MessageAttachment> MessageAttachments { get; set; }
    public DbSet<SystemMessage> SystemMessages { get; set; }

    public MessageContext(DbContextOptions<MessageContext> options)
        : base(options) { }

    public MessageContext(DbContextOptions<MessageContext> options, IMediator mediator)
        : base(options)
    {
        this.mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new SystemMessageConfiguration());
        modelBuilder.ApplyConfiguration(new MessageAttachmentConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(cancellationToken);
        await DispatchDomainEventsAsync();
        return result;
    }

    private async Task DispatchDomainEventsAsync()
    {
        List<EntityEntry<Entity>> domainEntities = ChangeTracker.Entries<Entity>().ToList();
        List<INotification> domainEvents = ChangeTracker
            .Entries<Entity>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        foreach (EntityEntry<Entity> entry in domainEntities)
        {
            entry.Entity.ClearDomainEvents();
        }

        if (mediator == null)
            return;

        foreach (INotification domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}
