using SchoolAccount.Application.Features.Academies;
using SchoolAccount.Application.Features.Academies.GetAcademies;

namespace SchoolAccount.Application.Abstractions.Clients;

public interface IAcademiesApiClient
{
    Task<GetAcademyEstablishmentResponse> GetEstablishmentDetails(
        string ukprn,
        CancellationToken cancellationToken
    );

    Task<GetAcademyTrustResponse> GetTrustDetails(
        string ukprn,
        CancellationToken cancellationToken
    );
}
