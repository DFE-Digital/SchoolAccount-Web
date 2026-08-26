using System.Net;
using NSubstitute;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Collect.CensusStatuses;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.SharedKernel;
using Shouldly;
using Action = SchoolAccount.Application.Collect.CensusStatuses.Action;

namespace SchoolAccount.IntegrationTests.Features.Dashboard;

public class DashboardControllerTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly SchoolAccountWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    private readonly IQueryHandler<
        GetCensusStatusesQuery,
        List<GetCensusStatusesResponse>
    > _getCensusStatusesHandler = Substitute.For<
        IQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>>
    >();

    public DashboardControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateAuthorisedClient(services =>
            services.AddScoped<
                IQueryHandler<GetCensusStatusesQuery, List<GetCensusStatusesResponse>>
            >(_ => _getCensusStatusesHandler)
        );
    }

    [Fact]
    public async Task Ensure_that_the_dashboard_controller_returns_correct_user_name()
    {
        // Arrange
        _getCensusStatusesHandler
            .Handle(Arg.Any<GetCensusStatusesQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(CreateNotInterestingCensusStatuses()));

        var pageUri = _factory.GeneratePath("Dashboard", "Dashboard");

        // Act
        var response = await _client.GetAsync(pageUri, TestContext.Current.CancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.ShouldNotBeNull();

        var pageTitle = page.GetTitle();
        pageTitle.ShouldNotBeNull();
        pageTitle.ShouldBeEquivalentTo("Dashboard");

        var headingElement = page.GetFirstHeading();
        headingElement.ShouldNotBeNull();
        headingElement.ShouldBeEquivalentTo(
            $"Hello {MockAuthHandler.FakeGivenName} {MockAuthHandler.FakeFamilyName}"
        );

        var bodyElement = page.GetFirstBody();
        bodyElement.ShouldNotBeNull();
        bodyElement.ShouldContainWithoutWhitespace("Test School");
    }

    [Fact]
    public async Task Ensure_that_the_dashboard_controller_returns_actions_when_there_are_actions()
    {
        // Arrange
        _getCensusStatusesHandler
            .Handle(Arg.Any<GetCensusStatusesQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(CreateInterestingCensusStatuses()));

        var pageUri = _factory.GeneratePath("Dashboard", "Dashboard");

        // Act
        var response = await _client.GetAsync(pageUri, TestContext.Current.CancellationToken);
        var page = await AngleSharpPage.FromResponseAsync<CommonPage>(
            response,
            TestContext.Current.CancellationToken
        );

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        page.ShouldNotBeNull();

        var bodyElement = page.GetFirstBody();
        bodyElement.ShouldNotBeNull();
        bodyElement.ShouldContainWithoutWhitespace("Test Action");
        bodyElement.ShouldContainWithoutWhitespace("Test Status");
    }

    private List<GetCensusStatusesResponse> CreateInterestingCensusStatuses() =>
        [
            new()
            {
                Id = "Test-id",
                Interesting = true,
                Actions =
                [
                    new Action
                    {
                        Name = "Test Action",
                        Status = new Status { Name = "Test Status" },
                    },
                ],
            },
        ];

    private List<GetCensusStatusesResponse> CreateNotInterestingCensusStatuses() =>
        [new() { Id = "Test-id", Interesting = false }];
}
