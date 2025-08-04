using MediatR;

namespace QuickChat.Attachment.Application.Queries;

public record GetAttachmentsByIdQuery(IEnumerable<Guid> Ids)
    : IRequest<IList<Domain.Entities.Attachment>>;
