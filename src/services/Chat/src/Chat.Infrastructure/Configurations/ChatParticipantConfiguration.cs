using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickChat.Chat.Domain.Entities;

namespace QuickChat.Chat.Infrastructure.Configurations;

class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
{
    public void Configure(EntityTypeBuilder<ChatParticipant> confuguration)
    {
        confuguration.Ignore(b => b.DomainEvents);
    }
}
