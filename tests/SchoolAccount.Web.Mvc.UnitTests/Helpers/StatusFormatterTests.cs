using SchoolAccount.Web.Mvc.Helpers;
using Shouldly;
using static SchoolAccount.Web.Mvc.Helpers.StatusFormatter;

namespace SchoolAccount.Web.Mvc.UnitTests.Helpers;

public class StatusFormatterTests
{
    private readonly StatusFormatter _statusFormatter = new();

    [Fact]
    public void An_unavailable_status_is_formatted_as_status_unavailable()
    {
        // Arrange
        var status = CollectUnavailableStatus;

        // Act
        var result = _statusFormatter.FormatStatus(status);

        // Assert
        result.ShouldBe(StatusUnavailableLabel);
    }

    [Fact]
    public void Any_status_that_is_not_unavailable_is_returned_directly()
    {
        // Arrange
        var status = "Test Status";

        // Act
        var result = _statusFormatter.FormatStatus(status);

        // Assert
        result.ShouldBe(status);
    }
}
