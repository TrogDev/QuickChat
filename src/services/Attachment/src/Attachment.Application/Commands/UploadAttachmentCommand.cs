using MediatR;
using QuickChat.Attachment.Domain.Enums;

namespace QuickChat.Attachment.Application.Commands;

public record UploadAttachmentCommand(
    string FileName,
    AttachmentType Type,
    Stream Stream,
    string ContentType,
    long Size
) : IRequest<Domain.Entities.Attachment> { }
