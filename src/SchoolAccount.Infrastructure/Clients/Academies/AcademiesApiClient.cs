using Microsoft.Extensions.Logging;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Features.Academies;

namespace SchoolAccount.Infrastructure.Clients.Academies;

public class AcademiesApiClient(HttpClient httpClient, ILogger<AcademiesApiClient> logger)
    : IAcademiesApiClient
{
    private const string? _getEstablishmentEndpoint = "establishment/";
    private const string _getTrustEndpoint = "trust/";

    public async Task<AcademyOrganisation?> GetOrganisationDetails(
        string ukPrn,
        CancellationToken cancellationToken
    ) { }

    public async Task<AcademyTrust?> GetTrustDetails(
        string ukPrn,
        CancellationToken cancellationToken
    ) { }
}
