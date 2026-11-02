using GovUK.Dfe.AcademiesApi.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using RichardSzalay.MockHttp;
using SchoolAccount.Infrastructure.Clients.Academies;
using Shouldly;
using static System.Net.HttpStatusCode;
using static System.Net.Mime.MediaTypeNames.Application;

namespace SchoolAccount.Infrastructure.UnitTests.Academies;

public class AcademiesApiClientTests : IDisposable
{
    private const string _baseAddress = "http://localhost";
    private const string _ukprn = "10012345";

    private const string _validValidationErrorResponse = """
        {
          "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
          "title": "One or more validation errors occurred.",
          "status": 400,
          "errors": {
            "Ukprn": [ "The Ukprn field is required." ]
          }
        }
        """;

    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private readonly FakeLogger<AcademiesApiClient> _logger = new();
    private readonly MockHttpMessageHandler _mockHttp = new();

    public void Dispose()
    {
        _mockHttp.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task A_trust_and_its_establishments_are_mapped_from_the_api_response()
    {
        // Arrange
        const string trustBody = """
            { "ukprn": "10012345", "name": "Test Trust", "groupUid": "TR00123" }
            """;

        const string establishmentsBody = """
            [ { "ukprn": "10011111", "name": "Test School" } ]
            """;

        _mockHttp.When($"{_baseAddress}/v4/trust/{_ukprn}").Respond(OK, Json, trustBody);
        _mockHttp
            .When($"{_baseAddress}/v4/establishments/trust")
            .Respond(OK, Json, establishmentsBody);
        var client = ClientRespondingWith();

        // Act
        var result = await client.GetTrustDetails(_ukprn, _cancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Ukprn.ShouldBe("10012345");
        result.Value.Name.ShouldBe("Test Trust");
        result.Value.GroupUid.ShouldBe("TR00123");
        result
            .Value.Establishments.ShouldHaveSingleItem()
            .EstablishmentName.ShouldBe("Test School");
    }

    [Fact]
    public async Task An_establishment_is_mapped_from_the_api_response()
    {
        // Arrange
        const string establishmentBody = """
            { "ukprn": "10011111", "name": "Test School", "urn": "100001" }
            """;

        _mockHttp
            .When($"{_baseAddress}/v4/establishment/10011111")
            .Respond(OK, Json, establishmentBody);
        var client = ClientRespondingWith();

        // Act
        var result = await client.GetEstablishmentDetails("10011111", _cancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Ukprn.ShouldBe("10011111");
        result.Value.EstablishmentName.ShouldBe("Test School");
        result.Value.Urn.ShouldBe("100001");
    }

    [Fact]
    public async Task A_validation_error_getting_a_trust_fails_the_request()
    {
        // Arrange
        _mockHttp
            .When($"{_baseAddress}/v4/trust/{_ukprn}")
            .Respond(BadRequest, ProblemJson, _validValidationErrorResponse);
        var client = ClientRespondingWith();

        // Act
        var result = await client.GetTrustDetails(_ukprn, _cancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Description.ShouldBe($"Failed to retrieve trust {_ukprn}");
    }

    [Fact]
    public async Task Validation_errors_are_logged()
    {
        // Arrange
        _mockHttp
            .When($"{_baseAddress}/v4/trust/{_ukprn}")
            .Respond(BadRequest, ProblemJson, _validValidationErrorResponse);
        var client = ClientRespondingWith();

        // Act
        var result = await client.GetTrustDetails(_ukprn, _cancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        _logger.Collector.Count.ShouldBe(1);
        _logger.Collector.LatestRecord.ShouldNotBeNull();
        _logger.Collector.LatestRecord.Level.ShouldBe(LogLevel.Error);
        _logger.Collector.LatestRecord.StructuredState.ShouldNotBeNull();
    }

    private AcademiesApiClient ClientRespondingWith()
    {
        var httpClient = _mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri(_baseAddress);

        var establishments = new EstablishmentsV4Client(_baseAddress, httpClient);
        var trusts = new TrustsV4Client(_baseAddress, httpClient);

        return new AcademiesApiClient(establishments, trusts, _logger);
    }
}
