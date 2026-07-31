using SchoolAccount.Web.Mvc.Models;
using Shouldly;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace SchoolAccount.Web.Mvc.UnitTests.Models;

public class ErrorViewModelTests
{
    [Fact]
    public void Is_not_found_when_status_code_is_404()
    {
        // Act
        var sut = new ErrorViewModel(Status404NotFound);
        
        // Assert
        sut.StatusCode.ShouldBe(Status404NotFound);
        sut.IsNotFound.ShouldBeTrue();
    }
    
    [Fact]
    public void Is_forbidden_when_status_code_is_403()
    {
        // Act
        var sut = new ErrorViewModel(Status403Forbidden);
        
        // Assert
        sut.StatusCode.ShouldBe(Status403Forbidden);
        sut.IsForbidden.ShouldBeTrue();
    }
}
