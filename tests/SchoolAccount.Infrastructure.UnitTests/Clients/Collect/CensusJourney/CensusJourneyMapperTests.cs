using SchoolAccount.Infrastructure.Clients.Collect.CensusJourney;
using Shouldly;

namespace SchoolAccount.Infrastructure.UnitTests.Clients.Collect.CensusJourney;

public class CensusJourneyMapperTests
{
    [Fact]
    public void Step_by_step_content_does_not_link_to_the_prototype()
    {
        // Arrange
        var apiResponse = new GetCensusJourneyApiResponse
        {
            Status = new GetCensusJourneyApiResponseStatus { Label = "Unavailable" },
            SupportService = new GetCensusJourneyApiResponseSupportService
            {
                Title = "Get help",
                Description = "Contact the census team",
                Url = new Uri("https://example.com/support"),
            },
            CallToAction = new GetCensusJourneyApiResponseCallToAction
            {
                Label = "Go to COLLECT",
                Url = new Uri("https://example.com/collect"),
            },
        };

        // Act
        var response = CensusJourneyMapper.ToResponse(apiResponse);

        // Assert
        response.StepByStep.ShouldAllBe(step => !step.Body.Contains("herokuapp.com"));
    }
}
