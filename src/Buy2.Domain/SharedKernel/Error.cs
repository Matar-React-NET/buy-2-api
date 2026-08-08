using System;
using System.Collections.Generic;
using System.Text;

namespace Buy2.Domain.SharedKernel;

public record Error(string Code, string Message)
{
    private const string NotFoundSuffix = ".NotFound";
    private const string ValidationPrefix = "Validation.";
    private const string ConflictCode = "Error.Conflict";
    private const string InternalCode = "Error.Internal";

    // Predefined error instances for common scenarios
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "Value cannot be null.");
    public static readonly Error NotAuthorized = new("Error.NotAuthorized", "User is not authorized to perform this action.");
    public static Error NotFound(string entity, object id) => new($"{entity}{NotFoundSuffix}", $"{entity} with ID {id} not found.");
    public static Error Conflict(string message) => new($"{ConflictCode}", message);
    public static Error Validation(string field, string message) => new($"{ValidationPrefix}{field}", message);
    public static Error Internal(string message) => new($"{InternalCode}", message);

    // Boolean properties to check the type of error
    public bool IsNotAuthorized => Code == NotAuthorized.Code;
    public bool IsNullValue => Code == NullValue.Code;
    public bool IsConflict => Code == ConflictCode;
    public bool IsInternal => Code == InternalCode;
    public bool IsNotFound => Code.EndsWith(NotFoundSuffix, StringComparison.OrdinalIgnoreCase);
    public bool IsValidationError => Code.StartsWith(ValidationPrefix, StringComparison.OrdinalIgnoreCase);
}
