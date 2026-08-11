using Buy2.Application.Abstractions.Messaging;
using Buy2.Domain.SharedKernel;
using FluentValidation;
using FluentValidation.Results;

namespace Buy2.Application.Abstractions.Decorators;

internal static class ValidationDecorator
{
    internal sealed class CommandHandler<TCommand>(
            IEnumerable<IValidator<TCommand>> validators,
            ICommandHandler<TCommand> innerHandler
        ) : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public async Task<Result> HandleAsync(TCommand command)
        {
            ValidationFailure[] failures = await ValidateAsync(command, validators);

            if (!failures.Any())
            {
                return await innerHandler.HandleAsync(command);
            }

            return CreateValidationError(failures);
        }
    }

    internal sealed class CommandHandler<TCommand, TResponse>(
            IEnumerable<IValidator<TCommand>> validators,
            ICommandHandler<TCommand, TResponse> innerHandler
        ) : ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> HandleAsync(TCommand command)
        {
            ValidationFailure[] failures = await ValidateAsync(command, validators);
            if (!failures.Any())
            {
                return await innerHandler.HandleAsync(command);
            }
            return CreateValidationError(failures);
        }
    }

    private static async Task<ValidationFailure[]> ValidateAsync<TCommand>(
        TCommand command,
        IEnumerable<IValidator<TCommand>> validators)
    {
        if (!validators.Any())
        {
            return [];
        }

        var context = new ValidationContext<TCommand>(command);

        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context)));

        ValidationFailure[] validationFailures = [.. validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)];

        return validationFailures;
    }

    private static ValidationError CreateValidationError(ValidationFailure[] validationFailures) =>
        new([.. validationFailures.Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))]);

}
