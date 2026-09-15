using SchoolAccount.SharedKernel.Authentication;
using Shouldly;
using static SchoolAccount.Application.Features.Collect.GetCensusJourney.GetCensusJourneyMapper;
using static SchoolAccount.TestCommon.Builders.CensusStatusesResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetAcademyTrustResponseBuilder.GetAcademyEstablishmentResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetAcademyTrustResponseBuilder.GetAcademyNameAndCodeResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyContentResponseBuilder;
using static SchoolAccount.TestCommon.Builders.GetCensusJourney.GetCensusJourneyResponseBuilder;

namespace SchoolAccount.ApplicationTests.Collect.GetCensusJourney;

public static class GetCensusJourneyMapperTests
{
    public class CreateGetCensusJourneyResponseTests
    {
        [Fact]
        public void The_content_is_mapped_onto_the_response()
        {
            // Arrange
            var content = ACensusJourneyContentResponse();

            // Act
            var result = AGetCensusJourneyResponse().WithContent(content).Build();

            // Assert
            result.Content.ShouldBeEquivalentTo(content.Build());
        }

        [Fact]
        public void The_statuses_are_mapped_onto_the_response()
        {
            // Arrange
            var first = ACensusStatusResponse();
            var second = ACensusStatusResponse();

            // Act
            var result = AGetCensusJourneyResponse().WithSchoolStatuses(first, second).Build();

            // Assert
            result.SchoolStatuses[0].ShouldBe(first.Build());
            result.SchoolStatuses[1].ShouldBe(second.Build());
        }

        [Fact]
        public void An_empty_status_list_is_mapped_onto_the_response()
        {
            // Act
            var result = AGetCensusJourneyResponse().Build();

            // Assert
            result.SchoolStatuses.ShouldBeEmpty();
        }
    }

    public class TrustEstablishmentToOrganisationTests
    {
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
            result.Id.ShouldBe("test-ukprn");
            result.Ukprn.ShouldBe("test-ukprn");
            result.Name.ShouldBe("Test Establishment");
            result.Category.Id.ShouldBe("001");
            result.Category.Name.ShouldBe("Establishment");
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
            result.LocalAuthority!.Id.ShouldBe("test-la-code");
            result.LocalAuthority.Code.ShouldBe("test-la-code");
            result.LocalAuthority.Name.ShouldBe("Test Local Authority");
        }

        [Fact]
        public void An_empty_local_authority_is_used_when_the_name_and_code_are_missing()
        {
            // Arrange
            var establishment = AnAcademyEstablishment().Build();

            // Act
            var result = TrustEstablishmentToOrganisation(establishment);

            // Assert
            result.LocalAuthority.ShouldBe(
                new LocalAuthority
                {
                    Id = string.Empty,
                    Name = string.Empty,
                    Code = string.Empty,
                }
            );
        }
    }
}
