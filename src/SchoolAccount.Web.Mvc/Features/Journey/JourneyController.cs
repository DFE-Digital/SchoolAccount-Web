using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.GetCensusJourney;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Features.Journey;

[Route("/{action}")]
[Authorize]
public class JourneyController(
    IUserContext userContext,
    IQueryHandler<GetCensusJourneyQuery, GetCensusJourneyResponse> getCensusJourneyHandler
) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Journey(CancellationToken cancellationToken)
    {
        var getCensusJourneyQuery = new GetCensusJourneyQuery();

        var gesCensusJourneyResponse = await getCensusJourneyHandler.Handle(
            getCensusJourneyQuery,
            cancellationToken
        );

        var journeyViewModel = JourneyViewModelBuilder.Build(
            userContext.Name,
            gesCensusJourneyResponse.Value
        );

        return View(journeyViewModel);
    }
}
