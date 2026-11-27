using SeatSnap.BuildingBlocks;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SeatSnap.UnitTests;

public class ResultTests
{
    private static readonly ResultError Boom = new("test.boom", "Something broke");

    [Fact]
    public void Success_has_no_error()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(ResultError.None, result.Error);
    }

    [Fact]
    public void Failure_carries_its_error()
    {
        var result = Result.Failure(Boom);

        Assert.True(result.IsFailure);
        Assert.Equal(Boom, result.Error);
    }

    [Fact]
    public void Success_with_value_exposes_the_value()
    {
        var result = Result.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Reading_the_value_of_a_failed_result_throws()
    {
        var result = Result.Failure<int>(Boom);

        Assert.Throws<InvalidOperationException>(() => result.Value);
    }
}