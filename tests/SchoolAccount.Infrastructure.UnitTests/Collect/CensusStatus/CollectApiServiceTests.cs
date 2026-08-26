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
        var result = await service.GetCensusStatus(
            EmptyQuery(),
            TestContext.Current.CancellationToken
        );

        // Assert
        var status = result.ShouldHaveSingleItem();
        status.Id.ShouldBe("123");
        status.Interesting.ShouldBeTrue();
        status.Actions.ShouldHaveSingleItem().Name.ShouldBe("Autumn School Census");
        status.Actions[0].Status.Name.ShouldBe("Not Started");
    }

    [Fact]
    public async Task Validation_errors_are_logged_and_thrown_as_an_http_exception()
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
        var action = async () =>
            await service.GetCensusStatus(EmptyQuery(), TestContext.Current.CancellationToken);

        // Assert
        await action.ShouldThrowAsync<HttpRequestException>();
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
    public async Task Null_response_is_thrown_as_an_exception()
    {
        // Arrange
        const string responseBody = "null";

        var service = ServiceRespondingWith(Accepted, Json, responseBody);

        // Act
        var action = async () =>
            await service.GetCensusStatus(EmptyQuery(), TestContext.Current.CancellationToken);

        // Assert
        await action.ShouldThrowAsync<Exception>();
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
