using FluentValidation;
using QuickChat.Chat.Application.Commands;

namespace QuickChat.Chat.Application.Validators;

public class CreateChatCommandValidator : AbstractValidator<CreateChatCommand>
{
    public CreateChatCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().WithMessage("The Name field is required");
    }
}
