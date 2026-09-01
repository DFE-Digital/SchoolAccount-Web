using SchoolAccount.Application.Services.GetServiceActionUrl;

namespace SchoolAccount.Web.Mvc.Features.Dashboard;

public static class DashboardViewModelBuilder
{
    public static DashboardViewModel Build(
        string user,
        IReadOnlyList<string> censusStatuses,
        GetServiceActionsResponse getServiceActionsResponse
    )
    {
        return new DashboardViewModel
        {
            User = user,
            CensusStatuses = censusStatuses,
            CallToActionUrl = getServiceActionsResponse.CallToActionUrl,
        };
    }
}
