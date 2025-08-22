using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using QuickChat.Logging;

namespace QuickChat.Chat.Application.Behaviors;

public class ValidatorBehavior<TRequest, TResponse>(
    ILogger<ValidatorBehavior<TRequest, TResponse>> logger,
    IEnumerable<IValidator<TRequest>> validators
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> logger = logger;
    private readonly IEnumerable<IValidator<TRequest>> validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (validators.Any())
        {
            Validate(request);
        }

        return await next();
    }

    private void Validate(TRequest request)
    {
        string typeName = request.GetGenericTypeName();

        logger.LogInformation("Validating command {CommandType}", typeName);

        List<FluentValidation.Results.ValidationFailure> failures = validators
            .Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(error => error != null)
            .ToList();

        if (failures.Count != 0)
        {
            logger.LogInformation(
                "Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}",
                typeName,
                request,
                failures
            );

            throw new ValidationException("Validation exception", failures);
        }
    }
}
