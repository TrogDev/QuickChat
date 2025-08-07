using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.Infrastructure.Configurations;

class MessageAttachmentConfiguration : IEntityTypeConfiguration<MessageAttachment>
{
    public void Configure(EntityTypeBuilder<MessageAttachment> confuguration)
    {
        confuguration.Ignore(b => b.DomainEvents);
    }
}
