using MediatR;
using Microsoft.Extensions.Logging;
using QuickChat.Message.Application.Exceptions;
using QuickChat.Message.Application.Repositories;
using QuickChat.Message.Application.Services;
using QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.Application.Commands;

public class AddMessageCommandHandler(
    IMessageRepository repository,
    IAttachmentService attachmentService,
    ILogger<AddMessageCommandHandler> logger
) : IRequestHandler<AddMessageCommand>
{
    private readonly IMessageRepository repository = repository;
    private readonly IAttachmentService attachmentService = attachmentService;
    private readonly ILogger<AddMessageCommandHandler> logger = logger;

    public async Task Handle(AddMessageCommand request, CancellationToken cancellationToken)
    {
        IList<MessageAttachment> attachments;

        try
        {
            attachments = await attachmentService.GetAttachments(request.AttachmentIds);
        }
        catch (AttachmentServiceException e)
        {
            logger.LogError(e, "Failed to get attachments from Attachment Service");
            throw;
        }

        Domain.Entities.Message message = Domain.Entities.Message.Create(
            request.ChatId,
            request.UserId,
            request.Text,
            attachments
        );

        repository.Add(message);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
