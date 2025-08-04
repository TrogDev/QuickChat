using MediatR;
using Microsoft.Extensions.Logging;
using QuickChat.Attachment.Application.DTO;
using QuickChat.Attachment.Application.Exceptions;
using QuickChat.Attachment.Application.Repositories;
using QuickChat.Attachment.Application.Services;

namespace QuickChat.Attachment.Application.Commands;

public class UploadAttachmentCommandHandler(
    IFileUploader fileUploader,
    IAttachmentRepository repository,
    IAttachmentTypeValidator validator,
    ILogger<UploadAttachmentCommandHandler> logger
) : IRequestHandler<UploadAttachmentCommand, Domain.Entities.Attachment>
{
    public async Task<Domain.Entities.Attachment> Handle(
        UploadAttachmentCommand request,
        CancellationToken cancellationToken
    )
    {
        validator.Validate(request.Type, request.FileName);

        UploadedFileInfo uploadedFile;

        try
        {
            uploadedFile = await fileUploader.Upload(
                new DTO.FileInfo(
                    request.Stream,
                    request.FileName,
                    request.ContentType,
                    request.Size
                )
            );
        }
        catch (FileUploadException e)
        {
            logger.LogError(e, "Failed to upload file");
            throw;
        }

        Domain.Entities.Attachment attachment =
            new()
            {
                FileName = request.FileName,
                Type = request.Type,
                Url = uploadedFile.Url,
                Size = request.Size
            };

        repository.Add(attachment);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return attachment;
    }
}
