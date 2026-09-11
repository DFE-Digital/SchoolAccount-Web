using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Collect.GetCensusJourney;
using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;
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
        var query = new GetCensusJourneyQuery
        {
            Id = userContext.Id!,
            EmailAddress = userContext.EmailAddress!,
            Organisations = [userContext.Organisation!],
            Ukprn = userContext.Organisation!.Ukprn,
        };

        var getCensusJourneyResponse = await getCensusJourneyHandler.Handle(
            query,
            cancellationToken
        );

        var journeyViewModel = JourneyViewModelBuilder.Build(
            userContext.Name,
            getCensusJourneyResponse.Value
        );

        return View(journeyViewModel);
    }
}
