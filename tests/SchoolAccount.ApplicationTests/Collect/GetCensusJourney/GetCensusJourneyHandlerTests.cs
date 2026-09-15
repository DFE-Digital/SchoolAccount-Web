using NSubstitute;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Features.Collect.CensusStatuses;
using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.SharedKernel;
using SchoolAccount.SharedKernel.Authentication;
using Shouldly;
using static SchoolAccount.TestCommon.Builders.CensusStatusesResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetAcademyTrustResponseBuilder.GetAcademyEstablishmentResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetAcademyTrustResponseBuilder.GetAcademyTrustResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyContentResponseBuilder;

namespace SchoolAccount.ApplicationTests.Collect.GetCensusJourney;

public class GetCensusJourneyHandlerTests
{
    private static readonly GetCensusJourneyQuery _matQuery = CreateMatQuery();

    private readonly IAcademiesApiClient _academiesApiClient =
        Substitute.For<IAcademiesApiClient>();

    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private readonly ICollectApiClient _collectApiClient = Substitute.For<ICollectApiClient>();

    public GetCensusJourneyHandlerTests()
    {
        MockGetAcademiesResponse(AnAcademyTrust().Build());
        MockCensusStatusesResponse(ACensusStatusResponse().Build());
        MockGetCensusJourneyContentResponse(ACensusJourneyContentResponse().Build());
    }

    [Fact]
    public async Task Academies_api_is_not_called_when_the_organisation_is_an_establishment_category()
    {
        // Arrange
        var handler = CreateHandler();

        // Act
        await handler.Handle(_matQuery, _cancellationToken);

        // Assert
        await _academiesApiClient.Received(1).GetTrustDetails(_matQuery.Ukprn!, _cancellationToken);
    }

    [Fact]
    public async Task Academies_api_is_not_called_when_the_organisation_ukprn_is_null()
    {
        // Arrange
        var query = new GetCensusJourneyQuery
        {
            Id = "test-user-id",
            EmailAddress = "test-user@example.com",
            Ukprn = null,
            Organisation = new Organisation
            {
                Id = "test-org-id",
                Name = "Test School",
                Category = new Category { Id = "010", Name = "Multi Academy Trust" },
            },
        };

        var handler = CreateHandler();

        // Act
        await handler.Handle(query, _cancellationToken);

        // Assert
        await _academiesApiClient
            .DidNotReceive()
            .GetTrustDetails(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Academies_api_is_bypassed_when_the_organisation_is_of_establishment_category()
    {
        var query = new GetCensusJourneyQuery
        {
            Id = "test-user-id",
            EmailAddress = "test-user@example.com",
            Ukprn = "1234567",
            Organisation = new Organisation
            {
                Id = "test-org-id",
                Name = "Test School",
                Category = new Category { Id = "001", Name = "Establishment" },
            },
        };
        var handler = CreateHandler();

        // Act
        await handler.Handle(query, _cancellationToken);

        // Assert
        await _academiesApiClient.DidNotReceive().GetTrustDetails(query.Ukprn, _cancellationToken);
    }

    [Fact]
    public async Task The_trusts_establishments_are_used_as_the_census_status_organisations()
    {
        // Arrange
        MockGetAcademiesResponse(
            AnAcademyTrust()
                .WithEstablishments(
                    AnAcademyEstablishment().WithName("First Establishment"),
                    AnAcademyEstablishment().WithName("Second Establishment")
                )
                .Build()
        );
        var handler = CreateHandler();

        // Act
        await handler.Handle(_matQuery, _cancellationToken);

        // Assert
        await _collectApiClient
            .Received(1)
            .GetCensusStatuses(
                _matQuery.Id,
                _matQuery.EmailAddress,
                Arg.Is<IReadOnlyList<Organisation>>(organisations =>
                    organisations.Count == 2
                    && organisations[0].Name == "First Establishment"
                    && organisations[1].Name == "Second Establishment"
                ),
                _cancellationToken
            );
    }

    [Fact]
    public async Task The_content_and_statuses_are_mapped_into_a_successful_response()
    {
        // Arrange
        var content = ACensusJourneyContentResponse().Build();
        var status = ACensusStatusResponse().Build();
        MockGetCensusJourneyContentResponse(content);
        MockCensusStatusesResponse(status);
        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(_matQuery, _cancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Content.ShouldBe(content);
        result.Value.SchoolStatuses.ShouldBe([status]);
    }

    private GetCensusJourneyHandler CreateHandler()
    {
        return new GetCensusJourneyHandler(_collectApiClient, _academiesApiClient);
    }

    private void MockGetAcademiesResponse(GetAcademyTrustResponse trust)
    {
        _academiesApiClient
            .GetTrustDetails(_matQuery.Ukprn!, _cancellationToken)
            .Returns(Result.Success(trust));
    }

    private void MockCensusStatusesResponse(params GetCensusStatusesResponse[] statuses)
    {
        _collectApiClient
            .GetCensusStatuses(
                _matQuery.Id,
                _matQuery.EmailAddress,
                Arg.Any<IReadOnlyList<Organisation>>(),
                _cancellationToken
            )
            .Returns(statuses.ToList());
    }

    private void MockGetCensusJourneyContentResponse(GetCensusJourneyContentResponse content)
    {
        _collectApiClient
            .GetCensusJourneyContent(
                _matQuery.Id,
                _matQuery.EmailAddress,
                Arg.Any<IReadOnlyList<Organisation>>(),
                _cancellationToken
            )
            .Returns(Result.Success(content));
    }

    private static GetCensusJourneyQuery CreateMatQuery()
    {
        return new GetCensusJourneyQuery
        {
            Id = "test-user-id",
            EmailAddress = "test-user@example.com",
            Ukprn = "12345678",
            Organisation = new Organisation
            {
                Id = "test-org-id",
                Name = "Test School",
                Category = new Category { Id = "010", Name = "Multi Academy Trust" },
            },
        };
    }
}
