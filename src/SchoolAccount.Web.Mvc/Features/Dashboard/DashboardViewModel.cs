using SchoolAccount.Application.Services.GetServiceActionUrl;

namespace SchoolAccount.Web.Mvc.Features.Dashboard;

public sealed class DashboardViewModel
{
    public string User { get; init; }
    public IReadOnlyList<string> CensusStatuses { get; init; }
    public string CallToActionUrl { get; init; }
};
