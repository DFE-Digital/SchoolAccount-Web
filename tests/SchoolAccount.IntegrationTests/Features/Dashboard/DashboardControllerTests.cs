using System.Net;
using NSubstitute;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Greetings.GetTimeSpecificHello;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.SharedKernel;
using SchoolAccount.Web.Mvc;
using SchoolAccount.Web.Mvc.Features.Dashboard;
using SchoolAccount.Web.Mvc.Features.Start;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Dashboard;

public class DashboardControllerTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    private readonly IQueryHandler<
        GetTimeSpecificHelloQuery,
        GetTimeSpecificHelloResponse
    > _getTimeSpecificHelloHandler = Substitute.For<
        IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
    >();

    public DashboardControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateAuthorisedClient(services =>
            services.AddScoped<
                IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
            >(_ => _getTimeSpecificHelloHandler)
        );
    }

    [Fact]
    public async Task Ensure_that_the_dashboard_controller_returns_correct_user_name()
    {
        // Arrange
        var stubbedGetSpecificHelloResponse = new GetTimeSpecificHelloResponse(
            GetTimeSpecificHelloHandler.Messages.Morning
        );

        _getTimeSpecificHelloHandler
            .Handle(Arg.Any<GetTimeSpecificHelloQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(stubbedGetSpecificHelloResponse));

        var pageUri = UrlBuilder.GeneratePath<DashboardController>(
            nameof(DashboardController.Dashboard)
        );

        // Act
        var response = await _client.GetAsync(pageUri, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var html = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var page = new AngleSharpPage(html);

        page.ShouldNotBeNull();

        var pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo("Dashboard");

        var headingElement = page.GetFirstHeading();
        headingElement.ShouldNotBeNull();
        headingElement.ShouldBeEquivalentTo(
            $"{GetTimeSpecificHelloHandler.Messages.Morning} {MockAuthHandler.FakeGivenName} {MockAuthHandler.FakeFamilyName}"
        );
    }
}
