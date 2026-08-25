using NSubstitute;
using SchoolAccount.Application.Collect.CensusStatus;
using Shouldly;
using Action = SchoolAccount.Application.Collect.CensusStatus.Action;

namespace SchoolAccount.ApplicationTests.Collect;

public class GetCensusStatusHandlerTests
{
    [Fact]
    public async Task Handler_returns_failed_result_if_no_interesting_census_statuses_are_found()
    {
        // Arrange
        var response = new GetCensusStatusResponse { Id = "Test-id", Interesting = false };

        var collectApiService = Substitute.For<ICollectApiService>();
        collectApiService.GetCensusStatus(Arg.Any<GetCensusStatusQuery>()).Returns([response]);
        var handler = new GetCensusStatusHandler(collectApiService);

        // Act
        var result = await handler.Handle(
            new GetCensusStatusQuery(new GetCensusStatusRequestModel()),
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public async Task Handler_returns_correct_response_when_an_interesting_status_is_found()
    {
        // Arrange
        var response = new GetCensusStatusResponse
        {
            Actions =
            [
                new Action
                {
                    Name = "Action 1",
                    Status = new Status { Name = "Status 1" },
                },
            ],
            Id = "Test-id",
            Interesting = true,
        };

        var collectApiService = Substitute.For<ICollectApiService>();
        collectApiService.GetCensusStatus(Arg.Any<GetCensusStatusQuery>()).Returns([response]);
        var handler = new GetCensusStatusHandler(collectApiService);

        // Act
        var result = await handler.Handle(
            new GetCensusStatusQuery(new GetCensusStatusRequestModel()),
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(response.Id);
        result.Value.Interesting.ShouldBe(response.Interesting);
        result.Value.Actions.ShouldNotBeNull();
        result.Value.Actions.Count.ShouldBe(response.Actions.Count);
        result.Value.Actions[0].Name.ShouldBe(response.Actions[0].Name);
        result.Value.Actions[0].Status.Name.ShouldBe(response.Actions[0].Status.Name);
    }

    [Fact]
    public async Task Handler_returns_first_result_when_two_interesting_statuses_are_found()
    {
        // Arrange
        var firstResponse = new GetCensusStatusResponse
        {
            Actions =
            [
                new Action
                {
                    Name = "Action 1",
                    Status = new Status { Name = "Status 1" },
                },
            ],
            Id = "Test-id",
            Interesting = true,
        };

        var secondResponse = new GetCensusStatusResponse
        {
            Actions =
            [
                new Action
                {
                    Name = "Action 1",
                    Status = new Status { Name = "Status 1" },
                },
            ],
            Id = "Test-id",
            Interesting = true,
        };

        var collectApiService = Substitute.For<ICollectApiService>();
        collectApiService
            .GetCensusStatus(Arg.Any<GetCensusStatusQuery>())
            .Returns([firstResponse, secondResponse]);
        var handler = new GetCensusStatusHandler(collectApiService);

        // Act
        var result = await handler.Handle(
            new GetCensusStatusQuery(new GetCensusStatusRequestModel()),
            TestContext.Current.CancellationToken
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(firstResponse.Id);
        result.Value.Interesting.ShouldBe(firstResponse.Interesting);
        result.Value.Actions.ShouldNotBeNull();
        result.Value.Actions.Count.ShouldBe(firstResponse.Actions.Count);
        result.Value.Actions[0].Name.ShouldBe(firstResponse.Actions[0].Name);
        result.Value.Actions[0].Status.Name.ShouldBe(firstResponse.Actions[0].Status.Name);
    }
}
