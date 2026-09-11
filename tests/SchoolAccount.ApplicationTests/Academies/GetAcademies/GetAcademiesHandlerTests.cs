using NSubstitute;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Features.Academies.GetAcademies;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using Shouldly;

namespace SchoolAccount.ApplicationTests.Academies.GetAcademies;

public class GetAcademiesHandlerTests
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    [Fact]
    public async Task The_query_ukprn_is_passed_to_the_client()
    {
        // Arrange
        var query = CreateQuery();
        var response = CreateResponse();
        var academiesApiClient = ClientReturning(query, response);
        var handler = new GetAcademiesHandler(academiesApiClient);

        // Act
        await handler.Handle(query, _cancellationToken);

        // Assert
        await academiesApiClient.Received(1).GetTrustDetails(query.Ukprn, _cancellationToken);
    }

    [Fact]
    public async Task The_clients_trust_is_returned_as_a_success()
    {
        // Arrange
        var query = CreateQuery();
        var response = CreateResponse();
        var handler = new GetAcademiesHandler(ClientReturning(query, response));

        // Act
        var result = await handler.Handle(query, _cancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(response);
    }

    private static GetAcademiesQuery CreateQuery() => new() { Ukprn = "10012345" };

    private static GetAcademyTrustResponse CreateResponse() =>
        new() { Ukprn = "10012345", Name = "Test Trust" };

    private IAcademiesApiClient ClientReturning(
        GetAcademiesQuery query,
        GetAcademyTrustResponse response
    )
    {
        var academiesApiClient = Substitute.For<IAcademiesApiClient>();
        academiesApiClient.GetTrustDetails(query.Ukprn, _cancellationToken).Returns(response);

        return academiesApiClient;
    }
}
