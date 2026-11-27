namespace SeatSnap.BuildingBlocks;

public sealed record ResultError(string Code, string Description)
{
    public static readonly ResultError None = new(string.Empty, string.Empty);
}

public class Result
{
    protected Result(bool isSuccess, ResultError error)
    {
        if (isSuccess && error != ResultError.None)
        {
            throw new InvalidOperationException("A successful result cannot carry an error.");
        }

        if (!isSuccess && error == ResultError.None)
        {
            throw new InvalidOperationException("A failed result must carry an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public ResultError Error { get; }

    public static Result Success() => new(true, ResultError.None);

    public static Result Failure(ResultError error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true, ResultError.None);

    public static Result<T> Failure<T>(ResultError error) => new(default, false, error);
}

public sealed class Result<T> : Result
{
    private readonly T? _value;

    internal Result(T? value, bool isSuccess, ResultError error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot read the value of a failed result.");
}