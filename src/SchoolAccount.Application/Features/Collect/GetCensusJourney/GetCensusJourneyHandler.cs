using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.SharedKernel;
using SchoolAccount.SharedKernel.Authentication;

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
        var censusJourneyContentResult = await collectApiClient.GetCensusJourneyContent(
            query.Id,
            query.EmailAddress,
            query.Organisations,
            cancellationToken
        );

        var academiesApiResult = await academiesApiClient.GetTrustDetails(
            query.Ukprn,
            cancellationToken
        );

        var organisations = query.Organisations;

        if (academiesApiResult.IsSuccess)
        {
            var academyEstablishments = academiesApiResult.Value.Establishments;

            if (academyEstablishments != null && academyEstablishments.Any())
            {
                var trustOrganisations = academyEstablishments
                    .Select(establishment => new Organisation
                    {
                        Name = establishment.EstablishmentName,
                        Ukprn = establishment.Ukprn,
                        EstablishmentNumber = establishment.EstablishmentNumber,
                        Category =
                            establishment.EstablishmentGroupType != null
                                ? new Category
                                {
                                    Id = establishment.EstablishmentGroupType.Code,
                                    Name = establishment.EstablishmentGroupType.Name,
                                }
                                : new Category(),
                        LocalAuthority = new LocalAuthority
                        {
                            Id = establishment.LocalAuthorityCode!,
                            Name = establishment.LocalAuthorityName!,
                            Code = establishment.LocalAuthorityCode!,
                        },
                    })
                    .ToList();

                organisations = trustOrganisations;
            }
        }

        var censusStatusesResult = await collectApiClient.GetCensusStatuses(
            query.Id,
            query.EmailAddress,
            organisations,
            cancellationToken
        );

        var getCensusJourney = new GetCensusJourneyResponse
        {
            Content = censusJourneyContentResult.Value,
            SchoolStatuses = censusStatusesResult,
        };

        return await Task.FromResult(getCensusJourney);
    }
}
