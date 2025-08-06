using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace QuickChat.Chat.Infrastructure.Configurations;

class ChatConfiguration : IEntityTypeConfiguration<Domain.Entities.Chat>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Chat> confuguration)
    {
        confuguration.Ignore(b => b.DomainEvents);
        confuguration.HasIndex(b => b.Code);
        confuguration.HasMany(b => b.Participants).WithOne();
    }
}
