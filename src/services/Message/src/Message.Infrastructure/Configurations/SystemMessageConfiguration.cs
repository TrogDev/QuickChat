using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.Infrastructure.Configurations;

class SystemMessageConfiguration : IEntityTypeConfiguration<SystemMessage>
{
    public void Configure(EntityTypeBuilder<SystemMessage> confuguration)
    {
        confuguration.Ignore(b => b.DomainEvents);
    }
}
