using System;
using Xunit;

namespace Calabonga.UnitOfWork.Tests;

public sealed class ExceptionHelperTests
{
    [Fact]
    public void GetMessages_WhenExceptionIsNull_ReturnsPlaceholder()
        => Assert.Equal("Exception is NULL", ExceptionHelper.GetMessages(null));

    [Fact]
    public void GetMessages_WithSingleException_ContainsItsMessage()
    {
        var message = ExceptionHelper.GetMessages(new InvalidOperationException("boom"));

        Assert.Contains("boom", message);
    }

    [Fact]
    public void GetMessages_WithNestedExceptions_ContainsEveryInnerMessage()
    {
        var exception = new Exception("outer", new Exception("middle", new Exception("inner")));

        var message = ExceptionHelper.GetMessages(exception);

        Assert.Contains("outer", message);
        Assert.Contains("middle", message);
        Assert.Contains("inner", message);
    }
}
