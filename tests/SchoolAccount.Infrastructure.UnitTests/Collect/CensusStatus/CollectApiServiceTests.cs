using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using RichardSzalay.MockHttp;
using SchoolAccount.Application.Collect.CensusStatus;
using SchoolAccount.Infrastructure.Collect.CensusStatus;
using Shouldly;
using static System.Net.HttpStatusCode;
using static System.Net.Mime.MediaTypeNames.Application;

namespace SchoolAccount.Infrastructure.UnitTests.Collect.CensusStatus;

public class CollectApiServiceTests
{
    private const string _baseAddress = "http://localhost";
    private readonly FakeLogger<CollectApiService> _logger = new();

    [Fact]
    public async Task GetCensusStatus_should_return_bad_request_for_empty_request()
    {
        // Arrange
        const string validationMessage = "The Email field is required.";
        const string responseBody = $$"""
            {
              "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
              "title": "One or more validation errors occurred.",
              "status": 400,
              "errors": {
                "Email": [
                  "{{validationMessage}}"
                ]
              }
            }
            """;

        using MockHttpMessageHandler mockHttp = new();

        mockHttp.When($"{_baseAddress}/status").Respond(BadRequest, ProblemJson, responseBody);

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(_baseAddress);

        var service = new CollectApiService(httpClient, _logger);
        var query = new GetCensusStatusQuery(new GetCensusStatusRequestModel());

        // Act
        var result = await service.GetCensusStatus(query);

        // Assert
        result.ShouldBeEmpty();
        _logger.Collector.LatestRecord.ShouldNotBeNull();
        _logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Error);
        _logger.Collector.LatestRecord.Message.ShouldBe(validationMessage);
    }
}
