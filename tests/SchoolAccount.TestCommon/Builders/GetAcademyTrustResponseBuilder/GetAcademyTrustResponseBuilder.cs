using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;

namespace SchoolAccount.TestCommon.Builders.GetAcademyTrustResponseBuilder;

public class GetAcademyTrustResponseBuilder
{
    private string _name = "Test Trust";
    private string _ukprn = "12345678";
    private List<GetAcademyEstablishmentResponse>? _establishments = [];

    public static GetAcademyTrustResponseBuilder AnAcademyTrust() => new();

    public GetAcademyTrustResponseBuilder WithUkprn(string ukprn)
    {
        _ukprn = ukprn;
        return this;
    }

    public GetAcademyTrustResponseBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public GetAcademyTrustResponseBuilder WithEstablishment(
        GetAcademyEstablishmentResponseBuilder builder
    )
    {
        _establishments!.Add(builder.Build());
        return this;
    }

    public GetAcademyTrustResponseBuilder WithEstablishments(
        params GetAcademyEstablishmentResponseBuilder[] builders
    )
    {
        foreach (var builder in builders)
        {
            _establishments!.Add(builder.Build());
        }

        return this;
    }

    public GetAcademyTrustResponseBuilder WithNullEstablishments()
    {
        _establishments = null;
        return this;
    }

    public GetAcademyTrustResponse Build() =>
        new()
        {
            Name = _name,
            Ukprn = _ukprn,
            Establishments = _establishments,
        };
}
