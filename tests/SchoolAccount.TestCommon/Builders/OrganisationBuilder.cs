using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.TestCommon.Builders;

/// <summary>
/// Builds the organisation a test user belongs to, using the same type the app reads from the
/// organisation claim. Defaults to a single school.
/// </summary>
public class OrganisationBuilder
{
    private string _id = "2E774B32-E4DB-445B-B915-736C777FF5A4";
    private string _name = "Test School";
    private Category _category = new()
    {
        Id = Organisation.EstablishmentCategoryId,
        Name = "Establishment",
    };
    private string? _ukprn = "10037611";
    private string? _establishmentNumber = "2091";
    private LocalAuthority? _localAuthority = new()
    {
        Id = "502EF2E9-2CA6-4905-9BF7-E80695BD5717",
        Name = "SUNDERLAND CITY METROPOLITAN BOROUGH COUNCIL",
        Code = "394",
    };

    public static OrganisationBuilder AnOrganisation() => new();

    public OrganisationBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public OrganisationBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public OrganisationBuilder WithUkprn(string? ukprn)
    {
        _ukprn = ukprn;
        return this;
    }

    /// <summary>
    /// A multi-academy trust. Its schools come from the Academies API, not from the claim.
    /// </summary>
    public OrganisationBuilder AsTrust()
    {
        _category = new Category { Id = "010", Name = "Multi Academy Trust" };
        _establishmentNumber = null;
        _localAuthority = null;
        return this;
    }

    public Organisation Build()
    {
        return new Organisation
        {
            Id = _id,
            Name = _name,
            Category = _category,
            Ukprn = _ukprn,
            EstablishmentNumber = _establishmentNumber,
            LocalAuthority = _localAuthority,
        };
    }
}
