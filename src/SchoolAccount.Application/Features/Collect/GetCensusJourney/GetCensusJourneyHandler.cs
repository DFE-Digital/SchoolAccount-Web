using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.SharedKernel;
using SchoolAccount.SharedKernel.Authentication;
using static SchoolAccount.Application.Features.Collect.GetCensusJourney.GetCensusJourneyMapper;
using static SchoolAccount.SharedKernel.Authentication.OrganisationCategory;

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
        var organisations = query.Organisations;

        if (organisations[0].IsTrust())
        {
            var academiesApiResult = await academiesApiClient.GetTrustDetails(
                query.Ukprn,
                cancellationToken
            );

            if (academiesApiResult.IsSuccess)
            {
                var academyEstablishments = academiesApiResult.Value.Establishments;

                if (academyEstablishments is not null && academyEstablishments.Any())
                {
                    organisations = academyEstablishments
                        .Select(TrustEstablishmentToOrganisation)
                        .ToList();
                }
            }
        }

        var censusStatusesResult = await collectApiClient.GetCensusStatuses(
            query.Id,
            query.EmailAddress,
            organisations,
            cancellationToken
        );

        var censusJourneyContentResult = await collectApiClient.GetCensusJourneyContent(
            query.Id,
            query.EmailAddress,
            query.Organisations,
            cancellationToken
        );

        var getCensusJourney = CreateGetCensusJourneyResponse(
            censusJourneyContentResult.Value,
            censusStatusesResult
        );

        return await Task.FromResult(getCensusJourney);
    }
}
