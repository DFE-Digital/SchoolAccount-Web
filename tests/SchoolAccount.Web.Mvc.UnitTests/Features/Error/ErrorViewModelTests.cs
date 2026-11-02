using SchoolAccount.Web.Mvc.Features.Error;
using Shouldly;
using static System.Net.HttpStatusCode;
using static SchoolAccount.Web.Mvc.Features.Error.ErrorViewModel;

namespace SchoolAccount.Web.Mvc.UnitTests.Features.Error;

public class ErrorViewModelTests
{
    [Fact]
    public void Is_not_found_when_status_code_is_404()
    {
        // Arrange & Act
        var sut = new ErrorViewModel(NotFound);

        // Assert
        sut.StatusCode.ShouldBe(NotFound);
        sut.IsNotFound.ShouldBeTrue();
    }

    [Fact]
    public void Is_forbidden_when_status_code_is_403()
    {
        // Arrange & Act
        var sut = new ErrorViewModel(Forbidden);

        // Assert
        sut.StatusCode.ShouldBe(Forbidden);
        sut.IsForbidden.ShouldBeTrue();
    }

    [Fact]
    public void Title_is_not_found_when_status_code_is_404()
    {
        // Arrange & Act
        var sut = new ErrorViewModel(NotFound);

        // Assert
        sut.Title.ShouldBe(NotFoundTitle);
    }

    [Fact]
    public void Title_is_forbidden_when_status_code_is_403()
    {
        // Arrange & Act
        var sut = new ErrorViewModel(Forbidden);

        // Assert
        sut.Title.ShouldBe(ForbiddenTitle);
    }

    [Fact]
    public void Title_is_error_for_all_other_status_codes()
    {
        // Arrange & Act
        var sut = new ErrorViewModel(PaymentRequired);

        // Assert
        sut.Title.ShouldBe(ErrorTitle);
    }
}
