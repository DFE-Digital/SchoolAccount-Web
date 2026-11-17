using GovUK.Dfe.AcademiesApi.Client.Contracts;

namespace SchoolAccount.Infrastructure.UnitTests.Common.Builders.Clients.Academies.GetAcademies;

public class EstablishmentDtoBuilder
{
    private string? _ukprn = "10011111";
    private string? _name = "Test School";
    private string? _urn = "100001";
    private string? _establishmentNumber = "1234";
    private string? _localAuthorityCode = "201";
    private string? _localAuthorityName = "Camden";
    private NameAndCodeDto? _establishmentType = new() { Name = "Academy", Code = "AC" };
    private NameAndCodeDto? _establishmentGroupType = new()
    {
        Name = "Academy sponsor led",
        Code = "ASL",
    };
    private NameAndCodeDto? _phaseOfEducation = new() { Name = "Primary", Code = "PR" };

    public static EstablishmentDtoBuilder AnEstablishmentDto() => new();

    public EstablishmentDtoBuilder WithUkprn(string? ukprn)
    {
        _ukprn = ukprn;
        return this;
    }

    public EstablishmentDtoBuilder WithName(string? name)
    {
        _name = name;
        return this;
    }

    public EstablishmentDtoBuilder WithUrn(string? urn)
    {
        _urn = urn;
        return this;
    }

    public EstablishmentDtoBuilder WithEstablishmentNumber(string? establishmentNumber)
    {
        _establishmentNumber = establishmentNumber;
        return this;
    }

    public EstablishmentDtoBuilder WithLocalAuthorityCode(string? localAuthorityCode)
    {
        _localAuthorityCode = localAuthorityCode;
        return this;
    }

    public EstablishmentDtoBuilder WithLocalAuthorityName(string? localAuthorityName)
    {
        _localAuthorityName = localAuthorityName;
        return this;
    }

    public EstablishmentDtoBuilder WithEstablishmentType(NameAndCodeDto? establishmentType)
    {
        _establishmentType = establishmentType;
        return this;
    }

    public EstablishmentDtoBuilder WithEstablishmentGroupType(
        NameAndCodeDto? establishmentGroupType
    )
    {
        _establishmentGroupType = establishmentGroupType;
        return this;
    }

    public EstablishmentDtoBuilder WithPhaseOfEducation(NameAndCodeDto? phaseOfEducation)
    {
        _phaseOfEducation = phaseOfEducation;
        return this;
    }

    public EstablishmentDto Build() =>
        new()
        {
            Ukprn = _ukprn,
            Name = _name,
            Urn = _urn,
            EstablishmentNumber = _establishmentNumber,
            LocalAuthorityCode = _localAuthorityCode,
            LocalAuthorityName = _localAuthorityName,
            EstablishmentType = _establishmentType,
            EstablishmentGroupType = _establishmentGroupType,
            PhaseOfEducation = _phaseOfEducation,
        };
}
