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
    private readonly IAcademiesApiClient _academiesApiClient =
        Substitute.For<IAcademiesApiClient>();

    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private readonly ICollectApiClient _collectApiClient = Substitute.For<ICollectApiClient>();
    private readonly GetCensusJourneyQuery _query = CreateQuery();

    public GetCensusJourneyHandlerTests()
    {
        MockGetAcademiesResponse(AnAcademyTrust().Build());
        MockCensusStatusesResponse(ACensusStatusResponse().Build());
        MockGetCensusJourneyContentResponse(ACensusJourneyContentResponse().Build());
    }

    [Theory]
    [InlineData("010", "Multi Academy Trust")]
    [InlineData("013", "Single Academy Trust")]
    public async Task Academies_api_is_called_when_an_organisation_has_establishments(
        string categoryId,
        string categoryName
    )
    {
        var query = new GetCensusJourneyQuery
        {
            Id = "test-user-id",
            EmailAddress = "test-user@example.com",
            Ukprn = "12345678",
            Organisations =
            [
                new Organisation
                {
                    Id = "test-org-id",
                    Name = "Test School",
                    Category = new Category { Id = categoryId, Name = categoryName },
                },
            ],
        };
        var handler = CreateHandler();

        // Act
        await handler.Handle(query, _cancellationToken);

        // Assert
        await _academiesApiClient.Received(1).GetTrustDetails(query.Ukprn, _cancellationToken);
    }

    [Theory]
    [InlineData("000", "Unknown")]
    [InlineData("001", "Establishment")]
    [InlineData("002", "Local Authority")]
    [InlineData("003", "Other Legacy Organisation")]
    [InlineData("004", "Early Years Setting")]
    [InlineData("008", "Other Stakeholder")]
    [InlineData("009", "Training Provider")]
    [InlineData("011", "Government")]
    [InlineData("012", "Other Gias Stakeholder")]
    [InlineData("050", "Software Supplier")]
    [InlineData("051", "Further Education")]
    public async Task Academies_api_is_bypassed_when_not_an_organisation_with_establishments(
        string categoryId,
        string categoryName
    )
    {
        var query = new GetCensusJourneyQuery
        {
            Id = "test-user-id",
            EmailAddress = "test-user@example.com",
            Ukprn = "12345678",
            Organisations =
            [
                new Organisation
                {
                    Id = "test-org-id",
                    Name = "Test School",
                    Category = new Category { Id = categoryId, Name = categoryName },
                },
            ],
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
        await handler.Handle(_query, _cancellationToken);

        // Assert
        await _collectApiClient
            .Received(1)
            .GetCensusStatuses(
                _query.Id,
                _query.EmailAddress,
                Arg.Is<IReadOnlyList<Organisation>>(organisations =>
                    organisations.Count == 2
                    && organisations[0].Name == "First Establishment"
                    && organisations[1].Name == "Second Establishment"
                ),
                _cancellationToken
            );
    }

    [Fact]
    public async Task The_query_organisations_are_used_when_the_academies_api_fails()
    {
        // Arrange
        var handler = CreateHandler();

        _academiesApiClient
            .GetTrustDetails(_query.Ukprn, _cancellationToken)
            .Returns(
                Result.Failure<GetAcademyTrustResponse>(Error.NotFound("test-error", "Test error"))
            );

        // Act
        await handler.Handle(_query, _cancellationToken);

        // Assert
        await _collectApiClient
            .Received(1)
            .GetCensusStatuses(
                _query.Id,
                _query.EmailAddress,
                _query.Organisations,
                _cancellationToken
            );
    }

    [Fact]
    public async Task The_query_organisations_are_used_when_the_trust_has_no_establishments()
    {
        // Arrange
        MockGetAcademiesResponse(AnAcademyTrust().WithEstablishments().Build());
        var handler = CreateHandler();

        // Act
        await handler.Handle(_query, _cancellationToken);

        // Assert
        await _collectApiClient
            .Received(1)
            .GetCensusStatuses(
                _query.Id,
                _query.EmailAddress,
                _query.Organisations,
                _cancellationToken
            );
    }

    [Fact]
    public async Task The_query_organisations_are_used_when_the_trust_establishments_are_null()
    {
        // Arrange
        MockGetAcademiesResponse(AnAcademyTrust().WithNullEstablishments().Build());
        var handler = CreateHandler();

        // Act
        await handler.Handle(_query, _cancellationToken);

        // Assert
        await _collectApiClient
            .Received(1)
            .GetCensusStatuses(
                _query.Id,
                _query.EmailAddress,
                _query.Organisations,
                _cancellationToken
            );
    }

    [Fact]
    public async Task Census_journey_content_is_requested_for_the_query_organisations()
    {
        // Arrange
        MockGetAcademiesResponse(
            AnAcademyTrust().WithEstablishments(AnAcademyEstablishment()).Build()
        );
        var handler = CreateHandler();

        // Act
        await handler.Handle(_query, _cancellationToken);

        // Assert
        await _collectApiClient
            .Received(1)
            .GetCensusJourneyContent(
                _query.Id,
                _query.EmailAddress,
                _query.Organisations,
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
        var result = await handler.Handle(_query, _cancellationToken);

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
            .GetTrustDetails(_query.Ukprn, _cancellationToken)
            .Returns(Result.Success(trust));
    }

    private void MockCensusStatusesResponse(params GetCensusStatusesResponse[] statuses)
    {
        _collectApiClient
            .GetCensusStatuses(
                _query.Id,
                _query.EmailAddress,
                Arg.Any<IReadOnlyList<Organisation>>(),
                _cancellationToken
            )
            .Returns(statuses.ToList());
    }

    private void MockGetCensusJourneyContentResponse(GetCensusJourneyContentResponse content)
    {
        _collectApiClient
            .GetCensusJourneyContent(
                _query.Id,
                _query.EmailAddress,
                Arg.Any<IReadOnlyList<Organisation>>(),
                _cancellationToken
            )
            .Returns(Result.Success(content));
    }

    private static GetCensusJourneyQuery CreateQuery()
    {
        return new GetCensusJourneyQuery
        {
            Id = "test-user-id",
            EmailAddress = "test-user@example.com",
            Ukprn = "12345678",
            Organisations =
            [
                new Organisation
                {
                    Id = "test-org-id",
                    Name = "Test School",
                    Category = new Category { Id = "010", Name = "Multi Academy Trust" },
                },
            ],
        };
    }
}
