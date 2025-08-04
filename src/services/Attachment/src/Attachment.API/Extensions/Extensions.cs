using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using QuickChat.Attachment.Application.Commands;
using QuickChat.Attachment.Application.Repositories;
using QuickChat.Attachment.Application.Services;
using QuickChat.Attachment.Infrastructure;
using QuickChat.Attachment.Infrastructure.Behaviors;
using QuickChat.Attachment.Infrastructure.Repositories;
using QuickChat.Attachment.Infrastructure.Services;
using QuickChat.EFHelper;

namespace QuickChat.Attachment.API.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection("MinioOptions"));
        MinioOptions minioOptions = builder
            .Configuration.GetRequiredSection("MinioOptions")
            .Get<MinioOptions>()!;

        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            AmazonS3Config config =
                new()
                {
                    ServiceURL = minioOptions.Url,
                    ForcePathStyle = true, // Required for MinIO
                    AuthenticationRegion = minioOptions.AuthenticationRegion
                };

            return new AmazonS3Client(minioOptions.Login, minioOptions.Password, config);
        });

        builder.Services.AddTransient<IFileUploader, MinioFileUploader>();
        builder.Services.AddTransient<IAttachmentTypeValidator, AttachmentTypeValidator>();

        builder.Services.AddDbContext<AttachmentContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
        });
        builder.Services.AddMigration<AttachmentContext>();
        builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(UploadAttachmentCommand).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
    }
}
