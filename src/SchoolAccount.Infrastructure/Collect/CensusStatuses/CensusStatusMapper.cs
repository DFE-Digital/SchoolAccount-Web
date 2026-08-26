using SchoolAccount.Application.Collect.CensusStatuses;
using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Infrastructure.Collect.CensusStatuses;

public static class CensusStatusMapper
{
    public static GetCensusStatusesApiRequest ToApiRequest(
        string id,
        string emailAddress,
        IReadOnlyList<Organisation> organisations
    ) =>
        new()
        {
            Id = id,
            Email = emailAddress,
            Organisations = organisations,
        };

    public static GetCensusStatusesResponse ToResponse(OrganisationResponse organisation) =>
        new()
        {
            Id = organisation.Id,
            Interesting = organisation.Interesting,
            Actions = organisation
                .Actions.Select(action => new CensusAction
                {
                    Name = action.Name,
                    Status = new CensusStatus { Name = action.Status.Name },
                })
                .ToList(),
        };
}
