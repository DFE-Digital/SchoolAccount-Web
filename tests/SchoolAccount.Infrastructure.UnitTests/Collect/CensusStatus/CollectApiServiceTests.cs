using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using RichardSzalay.MockHttp;
using SchoolAccount.Application.Collect.CensusStatus;
using SchoolAccount.Infrastructure.Collect.CensusStatus;
using Shouldly;
using static System.Net.HttpStatusCode;
using static System.Net.Mime.MediaTypeNames.Application;

namespace SchoolAccount.Infrastructure.UnitTests.Collect.CensusStatus;

public class CollectApiServiceTests : IDisposable
{
    private const string _baseAddress = "http://localhost";
    private readonly FakeLogger<CollectApiService> _logger = new();
    private readonly MockHttpMessageHandler _mockHttp = new();

    [Fact]
    public async Task Census_statuses_are_mapped_from_the_api_response()
    {
        // Arrange
        const string responseBody = """
            {
              "details": [
                {
                  "id": "123",
                  "interesting": true,
                  "actions": [
                    { "name": "Autumn School Census", "status": { "name": "Not Started" } }
                  ]
                }
              ]
            }
            """;

        var service = ServiceRespondingWith(OK, Json, responseBody);

        // Act
        var result = await service.GetCensusStatus(EmptyQuery());

        // Assert
        var status = result.ShouldHaveSingleItem();
        status.Id.ShouldBe("123");
        status.Interesting.ShouldBeTrue();
        status.Actions.ShouldHaveSingleItem().Name.ShouldBe("Autumn School Census");
        status.Actions[0].Status.Name.ShouldBe("Not Started");
    }

    [Fact]
    public async Task A_request_rejected_as_invalid_returns_no_statuses()
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

        var service = ServiceRespondingWith(BadRequest, ProblemJson, responseBody);

        // Act
        var result = await service.GetCensusStatus(EmptyQuery());

        // Assert
        result.ShouldBeEmpty();
        _logger.Collector.Count.ShouldBe(1);
        _logger.Collector.LatestRecord.ShouldNotBeNull();
        _logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Error);
        _logger.Collector.LatestRecord.StructuredState.ShouldNotBeNull();
        _logger.Collector.LatestRecord.StructuredState.ShouldContain(property =>
            property.Key == "ValidationErrorCount" && property.Value == "1"
        );
        _logger.Collector.LatestRecord.StructuredState.ShouldContain(property =>
            property.Key == "@ValidationErrors"
        );
    }

    [Fact]
    public async Task A_failure_in_the_collect_service_is_logged_with_its_detail()
    {
        // Arrange
        const string detail = "Something went wrong upstream.";
        const string responseBody = $$"""
            {
              "title": "An error occurred while processing your request.",
              "status": 500,
              "detail": "{{detail}}"
            }
            """;

        var service = ServiceRespondingWith(InternalServerError, ProblemJson, responseBody);

        // Act
        var result = await service.GetCensusStatus(EmptyQuery());

        // Assert
        result.ShouldBeEmpty();
        _logger.Collector.LatestRecord.ShouldNotBeNull();
        _logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Error);
        _logger.Collector.LatestRecord.Message.ShouldContain(detail);
        _logger.Collector.LatestRecord.Message.ShouldContain("500");
    }

    [Fact]
    public async Task An_error_response_without_problem_details_is_logged_with_its_status()
    {
        // Arrange
        var service = ServiceRespondingWith(BadGateway, "text/html", "<html>Bad gateway</html>");

        // Act
        var result = await service.GetCensusStatus(EmptyQuery());

        // Assert
        result.ShouldBeEmpty();
        _logger.Collector.LatestRecord.ShouldNotBeNull();
        _logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Error);
        _logger.Collector.LatestRecord.Message.ShouldContain("502");
    }

    [Fact]
    public async Task A_response_that_is_not_valid_json_returns_no_statuses()
    {
        // Arrange
        var service = ServiceRespondingWith(OK, Json, "{ not json");

        // Act
        var result = await service.GetCensusStatus(EmptyQuery());

        // Assert
        result.ShouldBeEmpty();
        _logger.Collector.LatestRecord.ShouldNotBeNull();
        _logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Error);
        _logger.Collector.LatestRecord.Message.ShouldContain("valid JSON");
    }

    [Fact]
    public async Task A_collect_service_that_cannot_be_reached_returns_no_statuses()
    {
        // Arrange
        _mockHttp.When($"{_baseAddress}/status").Throw(new HttpRequestException("No such host"));

        var service = new CollectApiService(CreateHttpClient(), _logger);

        // Act
        var result = await service.GetCensusStatus(EmptyQuery());

        // Assert
        result.ShouldBeEmpty();
        _logger.Collector.LatestRecord.ShouldNotBeNull();
        _logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Error);
        _logger.Collector.LatestRecord.Exception.ShouldBeOfType<HttpRequestException>();
    }

    [Fact]
    public async Task An_unexpected_failure_returns_no_statuses()
    {
        // Arrange
        _mockHttp.When($"{_baseAddress}/status").Throw(new InvalidOperationException("Boom"));

        var service = new CollectApiService(CreateHttpClient(), _logger);

        // Act
        var result = await service.GetCensusStatus(EmptyQuery());

        // Assert
        result.ShouldBeEmpty();
        _logger.Collector.LatestRecord.ShouldNotBeNull();
        _logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Error);
        _logger.Collector.LatestRecord.Message.ShouldContain("unexpectedly");
    }

    public void Dispose()
    {
        _mockHttp.Dispose();
        GC.SuppressFinalize(this);
    }

    private static GetCensusStatusQuery EmptyQuery() => new(new GetCensusStatusRequestModel());

    private CollectApiService ServiceRespondingWith(
        HttpStatusCode statusCode,
        string mediaType,
        string content
    )
    {
        _mockHttp.When($"{_baseAddress}/status").Respond(statusCode, mediaType, content);

        return new CollectApiService(CreateHttpClient(), _logger);
    }

    private HttpClient CreateHttpClient()
    {
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(_baseAddress);

        return httpClient;
    }
}
