using SchoolAccount.Application.Features.Collect.CensusStatuses;
using SchoolAccount.SharedKernel.Authentication;
using Shouldly;
using static SchoolAccount.Application.Features.Collect.GetCensusJourney.GetCensusJourneyMapper;
using static SchoolAccount.TestCommon.Builders.CensusStatusesResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetAcademyTrustResponseBuilder.GetAcademyEstablishmentResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyContentResponseBuilder;

namespace SchoolAccount.ApplicationTests.Features.Collect.GetCensusJourney;

public class GetCensusJourneyMapperTests
{
    [Fact]
    public void The_content_and_status_are_mapped_onto_the_response()
    {
        // Arrange
        var content = ACensusJourneyContentResponse().Build();
        var statuses = new List<GetCensusStatusesResponse> { ACensusStatusResponse().Build() };

        // Act
        var result = CreateGetCensusJourneyResponse(content, statuses);

        // Assert
        result.Content.ShouldBe(content);
        result.SchoolStatuses.ShouldBe(statuses);
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
        var result = TrustEstablishmentToOrganisation(establishment);

        // Assert
        result.ShouldSatisfyAllConditions(
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
        var result = TrustEstablishmentToOrganisation(establishment);

        // Assert
        result.ShouldSatisfyAllConditions(
            organisation => organisation.LocalAuthority!.Id.ShouldBe("test-la-code"),
            organisation => organisation.LocalAuthority!.Code.ShouldBe("test-la-code"),
            organisation => organisation.LocalAuthority!.Name.ShouldBe("Test Local Authority")
        );
    }

    [Fact]
    public void An_empty_local_authority_is_used_when_the_name_and_code_are_missing()
    {
        // Arrange
        var establishment = AnAcademyEstablishment().Build();

        // Act
        var result = TrustEstablishmentToOrganisation(establishment);

        // Assert
        result.LocalAuthority.ShouldBeEquivalentTo(
            new LocalAuthority
            {
                Id = string.Empty,
                Name = string.Empty,
                Code = string.Empty,
            }
        );
    }
}
