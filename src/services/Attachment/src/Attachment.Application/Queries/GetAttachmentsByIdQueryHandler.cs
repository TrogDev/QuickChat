using MediatR;
using QuickChat.Attachment.Application.Repositories;

namespace QuickChat.Attachment.Application.Queries;

public class GetAttachmentsByIdQueryHandler(IAttachmentRepository repository)
    : IRequestHandler<GetAttachmentsByIdQuery, IList<Domain.Entities.Attachment>>
{
    private readonly IAttachmentRepository repository = repository;

    public async Task<IList<Domain.Entities.Attachment>> Handle(
        GetAttachmentsByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        return await repository.FindByIdAsync(request.Ids);
    }
}
