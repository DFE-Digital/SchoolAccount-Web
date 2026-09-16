using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.SharedKernel;
using SchoolAccount.SharedKernel.Authentication;
using static SchoolAccount.Application.Features.Collect.GetCensusJourney.GetCensusJourneyMapper;

namespace SchoolAccount.Application.Features.Collect.GetCensusJourney;

public class GetCensusJourneyHandler(
    ICollectApiClient collectApiClient,
    IAcademiesApiClient academiesApiClient
) : IQueryHandler<GetCensusJourneyQuery, GetCensusJourneyResponse>
{
    public async Task<Result<GetCensusJourneyResponse>> Handle(
        GetCensusJourneyQuery query,
        CancellationToken cancellationToken
    )
    {
        var organisations = await GetOrganisations(query, cancellationToken);

        var censusStatusesResult = await collectApiClient.GetCensusStatuses(
            query.Id,
            query.EmailAddress,
            organisations,
            cancellationToken
        );

        var censusJourneyContentResult = await collectApiClient.GetCensusJourneyContent(
            query.Id,
            query.EmailAddress,
            organisations,
            cancellationToken
        );

        var getCensusJourney = CreateGetCensusJourneyResponse(
            censusJourneyContentResult.Value,
            censusStatusesResult
        );

        return await Task.FromResult(getCensusJourney);
    }

    private async Task<IReadOnlyList<Organisation>> GetOrganisations(
        GetCensusJourneyQuery query,
        CancellationToken cancellationToken
    )
    {
        if (query.Organisation.IsEstablishment)
        {
            return [query.Organisation];
        }

        var trustEstablishments = await GetEstablishmentsFromTrust(
            query.Organisation.Ukprn,
            cancellationToken
        );

        return trustEstablishments.Any() ? trustEstablishments : [query.Organisation];
    }

    private async Task<IReadOnlyList<Organisation>> GetEstablishmentsFromTrust(
        string? ukprn,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrEmpty(ukprn))
        {
            return [];
        }

        var result = await academiesApiClient.GetTrustDetails(ukprn, cancellationToken);
        if (!result.IsSuccess)
        {
            return [];
        }

        return result.Value.Establishments.Select(TrustEstablishmentToOrganisation).ToList();
    }
}
