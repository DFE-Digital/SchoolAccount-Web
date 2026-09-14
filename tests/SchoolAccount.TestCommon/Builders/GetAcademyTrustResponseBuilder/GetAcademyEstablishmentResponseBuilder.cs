using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;

namespace SchoolAccount.TestCommon.Builders.GetAcademyTrustResponseBuilder;

public class GetAcademyEstablishmentResponseBuilder
{
    private string _ukprn = "87654321";
    private string _establishmentName = "Test Establishment";
    private GetAcademyNameAndCodeResponse? _establishmentType;
    private string? _urn = "test-urn";
    private string? _localAuthorityCode;
    private string? _localAuthorityName;

    public static GetAcademyEstablishmentResponseBuilder AnAcademyEstablishment() => new();

    public GetAcademyEstablishmentResponseBuilder WithUkprn(string ukprn)
    {
        _ukprn = ukprn;
        return this;
    }

    public GetAcademyEstablishmentResponseBuilder WithName(string establishmentName)
    {
        _establishmentName = establishmentName;
        return this;
    }

    public GetAcademyEstablishmentResponseBuilder WithUrn(string? urn)
    {
        _urn = urn;
        return this;
    }

    public GetAcademyEstablishmentResponseBuilder WithLocalAuthorityCode(string? localAuthorityCode)
    {
        _localAuthorityCode = localAuthorityCode;
        return this;
    }

    public GetAcademyEstablishmentResponseBuilder WithLocalAuthorityName(string? localAuthorityName)
    {
        _localAuthorityName = localAuthorityName;
        return this;
    }

    public GetAcademyEstablishmentResponseBuilder WithEstablishmentType(
        GetAcademyNameAndCodeResponseBuilder builder
    )
    {
        _establishmentType = builder.Build();
        return this;
    }

    public GetAcademyEstablishmentResponse Build() =>
        new()
        {
            Ukprn = _ukprn,
            EstablishmentName = _establishmentName,
            Urn = _urn,
            EstablishmentType = _establishmentType,
            LocalAuthorityCode = _localAuthorityCode,
            LocalAuthorityName = _localAuthorityName,
        };
}
