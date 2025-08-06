using MediatR;
using QuickChat.Chat.Application.Exceptions;
using QuickChat.Chat.Application.Repositories;
using QuickChat.Chat.Application.Services;

namespace QuickChat.Chat.Application.Commands;

public class CreateChatCommandHandler(IChatRepository repository, ICodeGenerator codeGenerator)
    : IRequestHandler<CreateChatCommand, Domain.Entities.Chat>
{
    // TODO: maybe it's worth moving this to configuration
    private const int codeLength = 8;

    private readonly IChatRepository repository = repository;
    private readonly ICodeGenerator codeGenerator = codeGenerator;

    public async Task<Domain.Entities.Chat> Handle(
        CreateChatCommand request,
        CancellationToken cancellationToken
    )
    {
        Domain.Entities.Chat chat = new() { Name = request.Name, Code = await GetUniqCode() };
        repository.Add(chat);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return chat;
    }

    private async Task<string> GetUniqCode()
    {
        string code = codeGenerator.Generate(codeLength);

        try
        {
            await repository.GetByCode(code);
            return await GetUniqCode();
        }
        catch (EntityNotFoundException)
        {
            return code;
        }
    }
}
