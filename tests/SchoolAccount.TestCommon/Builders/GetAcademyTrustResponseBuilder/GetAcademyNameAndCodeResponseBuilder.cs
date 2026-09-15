using SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;

namespace SchoolAccount.TestCommon.Builders.GetAcademyTrustResponseBuilder;

public class GetAcademyNameAndCodeResponseBuilder
{
    private string _name = "Test Name";
    private string _code = "0123456789";

    public static GetAcademyNameAndCodeResponseBuilder AnAcademyNameAndCode() => new();

    public GetAcademyNameAndCodeResponseBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public GetAcademyNameAndCodeResponseBuilder WithCode(string code)
    {
        _code = code;
        return this;
    }

    public GetAcademyNameAndCodeResponse Build() => new() { Name = _name, Code = _code };
}
