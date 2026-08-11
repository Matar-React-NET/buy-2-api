namespace Buy2.Domain.SharedKernel;

public sealed record ValidationError(Error[] Errors)
    : Error(Validation("General", "One or more validation errors occurred."))
{
    public static ValidationError FromResults(IEnumerable<Result> results)
    {
        return new([.. results.Where(r => r.IsFailure && r.Error.IsValidationError).Select(r => r.Error)]);
    }

    public bool IsSuccess => Errors.Length == 0;
    public bool IsFailure => !IsSuccess;
}
