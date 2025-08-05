using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace QuickChat.EFHelper;

internal class MigrationHostedService<TContext>(
    IServiceProvider serviceProvider,
    Func<TContext, IServiceProvider, Task> seeder
) : BackgroundService
    where TContext : DbContext
{
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private readonly Func<TContext, IServiceProvider, Task> seeder = seeder;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return serviceProvider.MigrateDbContextAsync(seeder);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}
