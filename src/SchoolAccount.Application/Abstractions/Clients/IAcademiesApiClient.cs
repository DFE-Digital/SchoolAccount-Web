using SchoolAccount.Application.Features.Academies;

namespace SchoolAccount.Application.Abstractions.Clients;

public class IAcademiesApiClient
{
    Task<GetAcademyOrganisationResponse?> GetOrganisationDetails(
        string ukPrn,
        CancellationToken cancellationToken
    );

    Task<GetAcademyTrustResponse?> GetTrustDetails(
        string ukPrn,
        CancellationToken cancellationToken
    );
}
