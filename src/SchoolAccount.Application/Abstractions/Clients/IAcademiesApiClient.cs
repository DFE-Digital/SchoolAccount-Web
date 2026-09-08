using SchoolAccount.Application.Features.Academies;

namespace SchoolAccount.Application.Abstractions.Clients;

public class IAcademiesApiClient
{
    Task<AcademyOrganisation?> GetOrganisationDetails(
        string ukPrn,
        CancellationToken cancellationToken
    );

    Task<AcademyTrust?> GetTrustDetails(string ukPrn, CancellationToken cancellationToken);
}
