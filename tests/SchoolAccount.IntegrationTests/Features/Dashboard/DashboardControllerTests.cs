using System.Net;
using NSubstitute;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Collect.CensusStatus;
using SchoolAccount.Application.Greetings.GetTimeSpecificHello;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.SharedKernel;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Dashboard;

public class DashboardControllerTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly SchoolAccountWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    private readonly IQueryHandler<
        GetTimeSpecificHelloQuery,
        GetTimeSpecificHelloResponse
    > _getTimeSpecificHelloHandler = Substitute.For<
        IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
    >();

    private readonly IQueryHandler<GetCensusStatusQuery, GetCensusStatusResponse> _getCensusStatusHanlder = Substitute.For<
        IQueryHandler<GetCensusStatusQuery, GetCensusStatusResponse>
    >();

    public DashboardControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateAuthorisedClient(services =>
        {
            services.AddScoped<
                IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
            >(_ => _getTimeSpecificHelloHandler);
            services.AddScoped<IQueryHandler<GetCensusStatusQuery, GetCensusStatusResponse>>(_ => _getCensusStatusHanlder);
        });
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

        _getCensusStatusHanlder.Handle(Arg.Any<GetCensusStatusQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(new GetCensusStatusResponse { Name= "Test School", Status = new Status() { Name = "TestStatus" } }));
        
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
            $"{GetTimeSpecificHelloHandler.Messages.Morning} {MockAuthHandler.FakeGivenName} {MockAuthHandler.FakeFamilyName}"
        );
    }
}
