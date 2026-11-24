using GovUK.Dfe.AcademiesApi.Client.Contracts;

namespace SchoolAccount.Infrastructure.UnitTests.Common.Builders.Clients.Academies.GetAcademies;

public class TrustDtoBuilder
{
    private string? _ukprn = "10012345";
    private string? _name = "Test Trust";
    private string? _groupUid = "TR00123";
    private NameAndCodeDto? _type = new() { Name = "Multi-academy trust", Code = "010" };

    public static TrustDtoBuilder ATrustDto() => new();

    public TrustDtoBuilder WithUkprn(string? ukprn)
    {
        _ukprn = ukprn;
        return this;
    }

    public TrustDtoBuilder WithName(string? name)
    {
        _name = name;
        return this;
    }

    public TrustDtoBuilder WithGroupUid(string? groupUid)
    {
        _groupUid = groupUid;
        return this;
    }

    public TrustDtoBuilder WithType(NameAndCodeDto? type)
    {
        _type = type;
        return this;
    }

    public TrustDto Build() =>
        new()
        {
            Ukprn = _ukprn,
            Name = _name,
            GroupUid = _groupUid,
            Type = _type,
        };
}
