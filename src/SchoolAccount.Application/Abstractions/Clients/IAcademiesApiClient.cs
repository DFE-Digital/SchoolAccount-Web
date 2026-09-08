using SchoolAccount.Application.Features.Academies;

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
