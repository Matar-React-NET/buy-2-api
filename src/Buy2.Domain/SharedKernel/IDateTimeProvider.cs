namespace Buy2.Domain.SharedKernel;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
