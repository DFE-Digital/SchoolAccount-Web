using NSubstitute;
using SchoolAccount.Application.Collect.CensusStatuses;
using Shouldly;
using Action = SchoolAccount.Application.Collect.CensusStatuses.Action;

namespace SchoolAccount.ApplicationTests.Collect;

public class GetCensusStatusesHandlerTests
{
    [Fact]
    public async Task Handler_returns_correct_response()
    {
        // Arrange
        var response = new GetCensusStatusesResponse
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
            .GetCensusStatuses(
                Arg.Any<GetCensusStatusesQuery>(),
                TestContext.Current.CancellationToken
            )
            .Returns([response]);
        var handler = new GetCensusStatusesHandler(collectApiService);

        // Act
        var result = await handler.Handle(
            new GetCensusStatusesQuery(),
            TestContext.Current.CancellationToken
        );

        // Assert
        await collectApiService
            .Received(1)
            .GetCensusStatuses(
                Arg.Any<GetCensusStatusesQuery>(),
                TestContext.Current.CancellationToken
            );
        result.Value.Count.ShouldBe(1);
        var firstResult = result.Value[0];
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        firstResult.Id.ShouldBe(response.Id);
        firstResult.Interesting.ShouldBe(response.Interesting);
        firstResult.Actions.ShouldNotBeNull();
        firstResult.Actions.Count.ShouldBe(response.Actions.Count);
        firstResult.Actions[0].Name.ShouldBe(response.Actions[0].Name);
        firstResult.Actions[0].Status.Name.ShouldBe(response.Actions[0].Status.Name);
    }

    [Fact]
    public async Task Handler_returns_correct_response_with_multiple_responses()
    {
        // Arrange
        var firstResponse = new GetCensusStatusesResponse
        {
            Actions =
            [
                new Action
                {
                    Name = "Action 1",
                    Status = new Status { Name = "Status 1" },
                },
            ],
            Id = "Test-id-1",
            Interesting = true,
        };

        var secondResponse = new GetCensusStatusesResponse
        {
            Actions =
            [
                new Action
                {
                    Name = "Action 2",
                    Status = new Status { Name = "Status 2" },
                },
            ],
            Id = "Test-id-2",
            Interesting = true,
        };

        var collectApiService = Substitute.For<ICollectApiService>();
        collectApiService
            .GetCensusStatuses(
                Arg.Any<GetCensusStatusesQuery>(),
                TestContext.Current.CancellationToken
            )
            .Returns([firstResponse, secondResponse]);
        var handler = new GetCensusStatusesHandler(collectApiService);

        // Act
        var result = await handler.Handle(
            new GetCensusStatusesQuery(),
            TestContext.Current.CancellationToken
        );

        // Assert
        await collectApiService
            .Received(1)
            .GetCensusStatuses(
                Arg.Any<GetCensusStatusesQuery>(),
                TestContext.Current.CancellationToken
            );
        result.Value.Count.ShouldBe(2);
        var firstResult = result.Value[0];
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        firstResult.Id.ShouldBe(firstResponse.Id);
        firstResult.Interesting.ShouldBe(firstResponse.Interesting);
        firstResult.Actions.ShouldNotBeNull();
        firstResult.Actions.Count.ShouldBe(firstResponse.Actions.Count);
        firstResult.Actions[0].Name.ShouldBe(firstResponse.Actions[0].Name);
        firstResult.Actions[0].Status.Name.ShouldBe(firstResponse.Actions[0].Status.Name);
        var secondResult = result.Value[1];
        secondResult.Id.ShouldBe(secondResponse.Id);
        secondResult.Interesting.ShouldBe(secondResponse.Interesting);
        secondResult.Actions.ShouldNotBeNull();
        secondResult.Actions.Count.ShouldBe(secondResponse.Actions.Count);
        secondResult.Actions[0].Name.ShouldBe(secondResponse.Actions[0].Name);
        secondResult.Actions[0].Status.Name.ShouldBe(secondResponse.Actions[0].Status.Name);
    }
}
