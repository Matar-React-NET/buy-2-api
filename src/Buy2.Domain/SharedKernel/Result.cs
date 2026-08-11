using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Buy2.Domain.SharedKernel;

public record Result
{
    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; init; } = Error.None;

    protected Result(bool isSuccess, Error error)
    {
        switch (isSuccess)
        {
            case true when error == Error.None:
                IsSuccess = true;
                break;
            case false when error != Error.None:
                IsSuccess = false;
                Error = error;
                break;
            default:
                throw new InvalidOperationException("Invalid result state.");
        }
    }


    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static implicit operator Result(Error error) => Failure(error);

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);
}

public record Result<T> : Result
{
    private readonly T? _value;

    [NotNull]
    public T Value => IsSuccess ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    protected Result(bool isSuccess, Error error, T? value = default) : base(isSuccess, error)
    {
        _value = value;
    }

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(Error error) => Failure(error);
    public static Result<T> Success(T value) => new(true, Error.None, value);
    public static new Result<T> Failure(Error error) => new(false, error, default);
}
