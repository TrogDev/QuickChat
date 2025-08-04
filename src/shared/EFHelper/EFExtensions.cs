using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuickChat.Logging;

namespace QuickChat.EFHelper;

public static class EFExtensions
{
    private const string ActivitySourceName = "DbMigrations";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public static IServiceCollection AddMigration<TContext>(this IServiceCollection services)
        where TContext : DbContext => services.AddMigration<TContext>((_, _) => Task.CompletedTask);

    public static IServiceCollection AddMigration<TContext>(
        this IServiceCollection services,
        Func<TContext, IServiceProvider, Task> seeder
    )
        where TContext : DbContext
    {
        return services.AddHostedService(sp => new MigrationHostedService<TContext>(sp, seeder));
    }

    public static IServiceCollection AddMigration<TContext, TDbSeeder>(
        this IServiceCollection services
    )
        where TContext : DbContext
        where TDbSeeder : class, IDbSeeder<TContext>
    {
        services.AddScoped<IDbSeeder<TContext>, TDbSeeder>();
        return services.AddMigration<TContext>(
            (context, sp) => sp.GetRequiredService<IDbSeeder<TContext>>().SeedAsync(context)
        );
    }

    internal static async Task MigrateDbContextAsync<TContext>(
        this IServiceProvider services,
        Func<TContext, IServiceProvider, Task> seeder
    )
        where TContext : DbContext
    {
        using IServiceScope scope = services.CreateScope();
        IServiceProvider scopeServices = scope.ServiceProvider;
        ILogger<TContext> logger = scopeServices.GetRequiredService<ILogger<TContext>>();
        TContext context = scopeServices.GetService<TContext>();

        using Activity activity = ActivitySource.StartActivity(
            $"Migration operation {typeof(TContext).Name}"
        );

        try
        {
            logger.LogInformation(
                "Migrating database associated with context {DbContextName}",
                typeof(TContext).Name
            );

            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(() => InvokeSeeder(seeder, context, scopeServices));
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while migrating the database used on context {DbContextName}",
                typeof(TContext).Name
            );

            activity.SetExceptionTags(ex);

            throw;
        }
    }

    internal static async Task InvokeSeeder<TContext>(
        Func<TContext, IServiceProvider, Task> seeder,
        TContext context,
        IServiceProvider services
    )
        where TContext : DbContext
    {
        using var activity = ActivitySource.StartActivity($"Migrating {typeof(TContext).Name}");

        try
        {
            await context.Database.MigrateAsync();
            await seeder(context, services);
        }
        catch (Exception ex)
        {
            activity.SetExceptionTags(ex);

            throw;
        }
    }
}
