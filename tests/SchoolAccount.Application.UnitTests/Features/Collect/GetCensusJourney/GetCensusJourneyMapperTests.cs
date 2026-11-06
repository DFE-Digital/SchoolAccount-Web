using SchoolAccount.Application.Features.Collect.CensusStatuses;
using SchoolAccount.SharedKernel.Authentication;
using Shouldly;
using static SchoolAccount.Application.Features.Collect.GetCensusJourney.GetCensusJourneyMapper;
using static SchoolAccount.Application.UnitTests.Common.Builders.Features.Collect.GetCensusJourney.Responses.GetAcademyEstablishmentResponseBuilder;
using static SchoolAccount.TestCommon.Builders.Features.Collect.CensusStatuses.CensusStatusesResponseBuilder;
using static SchoolAccount.TestCommon.Builders.Features.Collect.GetCensusJourney.Responses.GetCensusJourneyContentResponseBuilder;

namespace SchoolAccount.Application.UnitTests.Features.Collect.GetCensusJourney;

public class GetCensusJourneyMapperTests
{
    [Fact]
    public void The_content_and_status_are_mapped_onto_the_response()
    {
        // Arrange
        var content = ACensusJourneyContentResponse().Build();
        var statuses = new List<GetCensusStatusesResponse> { ACensusStatusResponse().Build() };

        // Act
        var censusJourneyResponse = CreateGetCensusJourneyResponse(content, statuses);

        // Assert
        censusJourneyResponse.Content.ShouldBe(content);
        censusJourneyResponse.SchoolStatuses.ShouldBe(statuses);
    }

    [Fact]
    public void The_establishment_is_mapped_correctly()
    {
        // Arrange
        var establishment = AnAcademyEstablishment()
            .WithUkprn("test-ukprn")
            .WithName("Test Establishment")
            .Build();

        // Act
        var organisationResponse = TrustEstablishmentToOrganisation(establishment);

        // Assert
        organisationResponse.ShouldSatisfyAllConditions(
            organisation => organisation.Id.ShouldBe("test-ukprn"),
            organisation => organisation.Ukprn.ShouldBe("test-ukprn"),
            organisation => organisation.Name.ShouldBe("Test Establishment"),
            organisation => organisation.Category.Id.ShouldBe("001"),
            organisation => organisation.Category.Name.ShouldBe("Establishment")
        );
    }

    [Fact]
    public void The_local_authority_is_mapped_when_both_the_name_and_code_are_present()
    {
        // Arrange
        var establishment = AnAcademyEstablishment()
            .WithLocalAuthorityCode("test-la-code")
            .WithLocalAuthorityName("Test Local Authority")
            .Build();

        // Act
        var organisationResponse = TrustEstablishmentToOrganisation(establishment);

        // Assert
        organisationResponse.LocalAuthority.ShouldBe(
            new LocalAuthority
            {
                Id = "test-la-code",
                Name = "Test Local Authority",
                Code = "test-la-code",
            }
        );
    }

    [Fact]
    public void An_empty_local_authority_is_used_when_the_name_and_code_are_missing()
    {
        // Arrange
        var establishment = AnAcademyEstablishment().Build();

        // Act
        var organisationResponse = TrustEstablishmentToOrganisation(establishment);

        // Assert
        organisationResponse.LocalAuthority.ShouldBe(
            new LocalAuthority
            {
                Id = string.Empty,
                Name = string.Empty,
                Code = string.Empty,
            }
        );
    }
}
