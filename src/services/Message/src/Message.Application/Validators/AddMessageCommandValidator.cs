using FluentValidation;
using QuickChat.Message.Application.Commands;

namespace QuickChat.Message.Application.Validators;

public class AddMessageCommandValidator : AbstractValidator<AddMessageCommand>
{
    public AddMessageCommandValidator()
    {
        RuleFor(c => c.Text).NotEmpty().WithMessage("The Text field is required");
    }
}
