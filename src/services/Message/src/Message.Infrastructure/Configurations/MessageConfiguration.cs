using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace QuickChat.Message.Infrastructure.Configurations;

class MessageConfiguration : IEntityTypeConfiguration<Domain.Entities.Message>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Message> confuguration)
    {
        confuguration.Ignore(b => b.DomainEvents);
        confuguration.HasMany(b => b.Attachments).WithOne();
    }
}
