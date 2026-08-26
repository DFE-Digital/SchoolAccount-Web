using NSubstitute;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Collect.CensusStatuses;
using SchoolAccount.SharedKernel.Authentication;
using SchoolAccount.TestCommon.Builders;
using Shouldly;

namespace SchoolAccount.ApplicationTests.Collect;

/// <remarks>
/// The handler only unpacks the query onto the client, so that is all these cover. Every
/// integration test stubs <see cref="Application.Abstractions.Messaging.IQueryHandler{TQuery,
/// TResponse}"/>, which means this is the only place the real handler runs.
/// </remarks>
public class GetCensusStatusesHandlerTests
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task Query_values_are_passed_to_the_client()
    {
        // Arrange
        var query = CreateQuery();
        var response = CensusStatusesResponseBuilder.Create().Build();
        var collectApiClient = ClientReturning(query, response);
        var handler = new GetCensusStatusesHandler(collectApiClient);

        // Act
        await handler.Handle(query, _cancellationToken);

        // Assert
        await collectApiClient
            .Received(1)
            .GetCensusStatuses(
                query.Id,
                query.EmailAddress,
                query.Organisations,
                _cancellationToken
            );
    }

    [Fact]
    public async Task The_clients_census_statuses_are_returned_as_a_success()
    {
        // Arrange
        var query = CreateQuery();
        var response = CensusStatusesResponseBuilder.Create().Build();
        var handler = new GetCensusStatusesHandler(ClientReturning(query, response));

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe([response]);
    }

    private static GetCensusStatusesQuery CreateQuery() =>
        new()
        {
            Id = "test-user-id",
            EmailAddress = "test-user@example.com",
            Organisations = [new Organisation { Id = "test-org-id", Name = "Test School" }],
        };

    private ICollectApiClient ClientReturning(
        GetCensusStatusesQuery query,
        GetCensusStatusesResponse response
    )
    {
        var collectApiClient = Substitute.For<ICollectApiClient>();
        collectApiClient
            .GetCensusStatuses(
                query.Id,
                query.EmailAddress,
                query.Organisations,
                _cancellationToken
            )
            .Returns([response]);

        return collectApiClient;
    }
}
