using Shouldly;
using UnitTestsTypes.Domain.Common;

namespace UnitTestsTypes.UnitTests;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        var result = Result.Success();

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        var result = Result.Failure("validation_error", "Name is required");

        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error!.Code.ShouldBe("validation_error");
        result.Error.Message.ShouldBe("Name is required");
    }

    [Fact]
    public void GenericSuccess_ShouldStoreValue()
    {
        var result = Result<string>.Success("ok");

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("ok");
        result.Error.ShouldBeNull();
    }

    [Fact]
    public void GenericFailure_ShouldStoreErrorWithoutValue()
    {
        var result = Result<string>.Failure("validation_error", "Name is required");

        result.IsSuccess.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.Error!.Code.ShouldBe("validation_error");
    }
}
