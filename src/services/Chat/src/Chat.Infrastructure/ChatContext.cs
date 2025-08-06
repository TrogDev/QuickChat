using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using QuickChat.Chat.Application.Repositories;
using QuickChat.Chat.Domain.Entities;
using QuickChat.Chat.Infrastructure.Configurations;

namespace QuickChat.Chat.Infrastructure;

public class ChatContext : DbContext, IUnitOfWork
{
    private readonly IMediator mediator;

    public DbSet<Domain.Entities.Chat> Chats { get; set; }
    public DbSet<ChatParticipant> ChatParticipants { get; set; }

    public ChatContext(DbContextOptions<ChatContext> options)
        : base(options) { }

    public ChatContext(DbContextOptions<ChatContext> options, IMediator mediator)
        : base(options)
    {
        this.mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ChatConfiguration());
        modelBuilder.ApplyConfiguration(new ChatParticipantConfiguration());
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

        foreach (INotification domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}
