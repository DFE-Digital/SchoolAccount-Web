using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
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
