using NSubstitute;
using SchoolAccount.Application.Collect.CensusStatuses;
using SchoolAccount.SharedKernel.Authentication;
using Shouldly;

namespace SchoolAccount.ApplicationTests.Collect;

public class GetCensusStatusesHandlerTests
{
    private static GetCensusStatusesQuery CreateQuery() =>
        new()
        {
            Id = "test-user-id",
            EmailAddress = "test-user@example.com",
            Organisations = [new Organisation { Id = "test-org-id", Name = "Test School" }],
        };

    [Fact]
    public async Task Handler_returns_correct_response()
    {
        // Arrange
        var response = new GetCensusStatusesResponse
        {
            Actions =
            [
                new CensusAction
                {
                    Name = "Action 1",
                    Status = new CensusStatus { Name = "Status 1" },
                },
            ],
            Id = "Test-id",
            Interesting = true,
        };

        var query = CreateQuery();
        var collectApiClient = Substitute.For<ICollectApiClient>();
        collectApiClient
            .GetCensusStatuses(
                query.Id,
                query.EmailAddress,
                query.Organisations,
                TestContext.Current.CancellationToken
            )
            .Returns([response]);
        var handler = new GetCensusStatusesHandler(collectApiClient);

        // Act
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        await collectApiClient
            .Received(1)
            .GetCensusStatuses(
                query.Id,
                query.EmailAddress,
                query.Organisations,
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
                new CensusAction
                {
                    Name = "Action 1",
                    Status = new CensusStatus { Name = "Status 1" },
                },
            ],
            Id = "Test-id-1",
            Interesting = true,
        };

        var secondResponse = new GetCensusStatusesResponse
        {
            Actions =
            [
                new CensusAction
                {
                    Name = "Action 2",
                    Status = new CensusStatus { Name = "Status 2" },
                },
            ],
            Id = "Test-id-2",
            Interesting = true,
        };

        var query = CreateQuery();
        var collectApiClient = Substitute.For<ICollectApiClient>();
        collectApiClient
            .GetCensusStatuses(
                query.Id,
                query.EmailAddress,
                query.Organisations,
                TestContext.Current.CancellationToken
            )
            .Returns([firstResponse, secondResponse]);
        var handler = new GetCensusStatusesHandler(collectApiClient);

        // Act
        var result = await handler.Handle(query, TestContext.Current.CancellationToken);

        // Assert
        await collectApiClient
            .Received(1)
            .GetCensusStatuses(
                query.Id,
                query.EmailAddress,
                query.Organisations,
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
