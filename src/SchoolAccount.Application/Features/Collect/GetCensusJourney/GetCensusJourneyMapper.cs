using SchoolAccount.Application.Features.Collect.CensusStatuses;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Application.Features.Collect.GetCensusJourney;

public static class GetCensusJourneyMapper
{
    public static GetCensusJourneyResponse CreateGetCensusJourneyResponse(
        GetCensusJourneyContentResponse censusJourneyContentResult,
        List<GetCensusStatusesResponse> censusStatusesResult
    )
    {
        return new GetCensusJourneyResponse
        {
            Content = censusJourneyContentResult,
            SchoolStatuses = censusStatusesResult,
        };
    }

    public static Organisation TrustEstablishmentToOrganisation(
        GetAcademyEstablishmentResponse establishment
    )
    {
        return new Organisation
        {
            Id = establishment.Ukprn,
            Name = establishment.EstablishmentName,
            Ukprn = establishment.Ukprn,
            EstablishmentNumber = establishment.EstablishmentNumber,
            Category = new Category { Id = "001", Name = "Establishment" },
            LocalAuthority = establishment
                is { LocalAuthorityName: not null, LocalAuthorityCode: not null }
                ? new LocalAuthority
                {
                    Id = establishment.LocalAuthorityCode,
                    Name = establishment.LocalAuthorityName,
                    Code = establishment.LocalAuthorityCode,
                }
                : new LocalAuthority(),
        };
    }
}
