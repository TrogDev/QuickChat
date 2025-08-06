using FluentValidation;
using QuickChat.Chat.Application.Commands;

namespace QuickChat.Chat.Application.Validators;

public class JoinChatCommandValidator : AbstractValidator<JoinChatCommand>
{
    public JoinChatCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().WithMessage("The Name field is required");
        RuleFor(c => c.Code).NotEmpty().WithMessage("The Code field is required");
    }
}
