using Microsoft.EntityFrameworkCore;

namespace QuickChat.EFHelper;

public interface IDbSeeder<in TContext>
    where TContext : DbContext
{
    Task SeedAsync(TContext context);
}
