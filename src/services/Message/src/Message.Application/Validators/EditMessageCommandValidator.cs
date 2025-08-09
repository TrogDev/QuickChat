using FluentValidation;
using QuickChat.Message.Application.Commands;

namespace QuickChat.Message.Application.Validators;

public class EditMessageCommandValidator : AbstractValidator<EditMessageCommand>
{
    public EditMessageCommandValidator()
    {
        RuleFor(c => c.Text).NotEmpty().WithMessage("The Text field is required");
    }
}
