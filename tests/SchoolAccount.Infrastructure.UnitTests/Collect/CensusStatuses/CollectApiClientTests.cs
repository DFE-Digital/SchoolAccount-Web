using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using RichardSzalay.MockHttp;
using SchoolAccount.Application.Collect.CensusStatuses;
using SchoolAccount.Infrastructure.Collect.CensusStatuses;
using Shouldly;
using static System.Net.HttpStatusCode;
using static System.Net.Mime.MediaTypeNames.Application;

namespace SchoolAccount.Infrastructure.UnitTests.Collect.CensusStatuses;

public class CollectApiClientTests : IDisposable
{
    private const string _baseAddress = "http://localhost";
    private const string _nullResponse = "null";

    private const string _validationErrorResponse = """
        {
          "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
          "title": "One or more validation errors occurred.",
          "status": 400,
          "errors": {
            "Email": [
              "The Email field is required."
            ]
          }
        }
        """;

    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private readonly FakeLogger<CollectApiClient> _logger = new();
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

        var service = ClientRespondingWith(OK, Json, responseBody);

        // Act
        var result = await service.GetCensusStatuses(EmptyQuery(), _cancellationToken);

        // Assert
        var status = result.ShouldHaveSingleItem();
        status.Id.ShouldBe("123");
        status.Interesting.ShouldBeTrue();
        status.Actions.ShouldHaveSingleItem().Name.ShouldBe("Autumn School Census");
        status.Actions[0].Status.Name.ShouldBe("Not Started");
    }

    [Fact]
    public async Task A_validation_error_response_fails_the_request()
    {
        // Arrange
        var service = ClientRespondingWith(BadRequest, ProblemJson, _validationErrorResponse);

        // Act
        var act = async () => await service.GetCensusStatuses(EmptyQuery(), _cancellationToken);

        // Assert
        await act.ShouldThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task Validation_errors_are_logged()
    {
        // Arrange
        var service = ClientRespondingWith(BadRequest, ProblemJson, _validationErrorResponse);

        // Act
        var act = async () => await service.GetCensusStatuses(EmptyQuery(), _cancellationToken);
        await act.ShouldThrowAsync<HttpRequestException>();

        // Assert
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
    public async Task An_empty_response_fails_the_request()
    {
        // Arrange
        var service = ClientRespondingWith(Accepted, Json, _nullResponse);

        // Act
        var act = async () => await service.GetCensusStatuses(EmptyQuery(), _cancellationToken);

        // Assert
        await act.ShouldThrowAsync<Exception>();
    }

    [Fact]
    public async Task An_empty_response_is_logged()
    {
        // Arrange
        var service = ClientRespondingWith(Accepted, Json, _nullResponse);

        // Act
        var act = async () => await service.GetCensusStatuses(EmptyQuery(), _cancellationToken);
        await act.ShouldThrowAsync<Exception>();

        // Assert
        _logger.Collector.Count.ShouldBe(1);
        _logger.Collector.LatestRecord.ShouldNotBeNull();
        _logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Error);
        _logger.Collector.LatestRecord.StructuredState.ShouldNotBeNull();
        _logger.LatestRecord.Message.ShouldContain("Response from /status was empty");
    }

    public void Dispose()
    {
        _mockHttp.Dispose();
        GC.SuppressFinalize(this);
    }

    private static GetCensusStatusesQuery EmptyQuery() => new();

    private CollectApiClient ClientRespondingWith(
        HttpStatusCode statusCode,
        string mediaType,
        string content
    )
    {
        _mockHttp.When($"{_baseAddress}/status").Respond(statusCode, mediaType, content);

        return new CollectApiClient(CreateHttpClient(), _logger);
    }

    private HttpClient CreateHttpClient()
    {
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(_baseAddress);

        return httpClient;
    }
}
