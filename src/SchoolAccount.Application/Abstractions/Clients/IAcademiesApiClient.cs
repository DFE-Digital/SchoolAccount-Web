using SchoolAccount.Application.Features.Academies.GetAcademies;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Abstractions.Clients;

public interface IAcademiesApiClient
{
    Task<Result<GetAcademyEstablishmentResponse>> GetEstablishmentDetails(
        string ukprn,
        CancellationToken cancellationToken
    );

    Task<Result<GetAcademyTrustResponse>> GetTrustDetails(
        string ukprn,
        CancellationToken cancellationToken
    );
}
