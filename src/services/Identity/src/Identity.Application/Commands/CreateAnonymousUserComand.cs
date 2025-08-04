using MediatR;
using QuickChat.Identity.Domain.Entities;

namespace QuickChat.Identity.Application.Commands;

public record CreateAnonymousUserCommand() : IRequest<UserCredentials>;
