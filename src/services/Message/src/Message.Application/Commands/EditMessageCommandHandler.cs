using MediatR;
using Microsoft.Extensions.Logging;
using QuickChat.Message.Application.Exceptions;
using QuickChat.Message.Application.Repositories;
using QuickChat.Message.Application.Services;
using QuickChat.Message.Domain.Entities;

namespace QuickChat.Message.Application.Commands;

public class EditMessageCommandHandler(
    IMessageRepository repository,
    IAttachmentService attachmentService,
    ILogger<EditMessageCommandHandler> logger
) : IRequestHandler<EditMessageCommand>
{
    private readonly IMessageRepository repository = repository;
    private readonly IAttachmentService attachmentService = attachmentService;
    private readonly ILogger<EditMessageCommandHandler> logger = logger;

    public async Task Handle(EditMessageCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.Message message = await repository.FindByIdAsync(request.Id);

        IList<MessageAttachment> attachments;

        try
        {
            attachments = await GetNewAttachments(message.Attachments, request.AttachmentIds);
        }
        catch (AttachmentServiceException e)
        {
            logger.LogError(e, "Failed to get attachments from Attachment Service");
            throw;
        }

        if (request.ActorId == null)
        {
            message.Edit(request.Text, attachments);
        }
        else
        {
            message.Edit(request.ActorId.Value, request.Text, attachments);
        }

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async ValueTask<IList<MessageAttachment>> GetNewAttachments(
        List<MessageAttachment> oldAttachments,
        IEnumerable<Guid> newAttachmentIds
    )
    {
        oldAttachments = [.. oldAttachments];
        List<Guid> toFetch = [];

        foreach (Guid id in newAttachmentIds)
        {
            if (!oldAttachments.Any(a => a.Id == id))
            {
                toFetch.Add(id);
            }
        }

        foreach (MessageAttachment attachment in oldAttachments.ToList())
        {
            if (!newAttachmentIds.Contains(attachment.Id))
            {
                oldAttachments.Remove(attachment);
            }
        }

        if (toFetch.Count == 0)
        {
            return oldAttachments;
        }

        oldAttachments.AddRange(await attachmentService.GetAttachments(toFetch));
        return oldAttachments;
    }
}
