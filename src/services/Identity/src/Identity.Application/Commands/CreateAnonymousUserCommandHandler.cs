using MediatR;
using QuickChat.Identity.Application.Services;
using QuickChat.Identity.Domain.Entities;

namespace QuickChat.Identity.Application.Commands;

public class CreateAnonymousUserCommandHandler(ITokenService tokenService)
    : IRequestHandler<CreateAnonymousUserCommand, UserCredentials>
{
    public Task<UserCredentials> Handle(
        CreateAnonymousUserCommand request,
        CancellationToken cancellationToken
    )
    {
        Guid userId = Guid.NewGuid();
        string accessToken = tokenService.CreateAccessToken(userId);
        UserCredentials result =
            new()
            {
                User = new() { Id = userId },
                AccessToken = accessToken
            };
        return Task.FromResult(result);
    }
}
