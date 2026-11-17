using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Features.Journey;

[Route("/{action}")]
[Authorize]
public class JourneyController(
    IUserContext userContext,
    IQueryHandler<GetCensusJourneyQuery, GetCensusJourneyResponse> getCensusJourneyHandler,
    IDateTimeProvider dateTimeProvider,
    IFeatureManager featureManager
) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Journey(CancellationToken cancellationToken)
    {
        var query = new GetCensusJourneyQuery
        {
            Id = userContext.Id!,
            EmailAddress = userContext.EmailAddress!,
            Organisation = userContext.Organisation!,
        };

        var getCensusJourneyResponse = await getCensusJourneyHandler.Handle(
            query,
            cancellationToken
        );

        var builder = new JourneyViewModelBuilder(dateTimeProvider, featureManager);
        var journeyViewModel = builder.Build(
            userContext.Name,
            getCensusJourneyResponse.Value,
            userContext.Organisation!
        );

        return View(journeyViewModel);
    }
}
