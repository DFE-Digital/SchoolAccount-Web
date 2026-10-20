using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.TestCommon.Builders;

/// <summary>
/// Builds the organisation a test user belongs to, using the same type the app reads from the
/// organisation claim. Start from <see cref="ASingleSchool"/> or <see cref="ATrust"/>, so every
/// test says which kind of organisation it uses.
/// </summary>
public class OrganisationBuilder
{
    private string _id = "2E774B32-E4DB-445B-B915-736C777FF5A4";
    private string _name;
    private string? _ukprn = "10037611";
    private readonly Category _category;
    private readonly string? _establishmentNumber;
    private readonly LocalAuthority? _localAuthority;

    private OrganisationBuilder(
        string name,
        Category category,
        string? establishmentNumber,
        LocalAuthority? localAuthority
    )
    {
        _name = name;
        _category = category;
        _establishmentNumber = establishmentNumber;
        _localAuthority = localAuthority;
    }

    /// <summary>A single school, the same as the default test user's organisation.</summary>
    public static OrganisationBuilder ASingleSchool() =>
        new(
            "Test School",
            new Category { Id = Organisation.EstablishmentCategoryId, Name = "Establishment" },
            "2091",
            new LocalAuthority
            {
                Id = "502EF2E9-2CA6-4905-9BF7-E80695BD5717",
                Name = "SUNDERLAND CITY METROPOLITAN BOROUGH COUNCIL",
                Code = "394",
            }
        );

    /// <summary>
    /// A multi-academy trust. Its schools come from the Academies API, not from the claim.
    /// </summary>
    public static OrganisationBuilder ATrust() =>
        new(
            "Test Multi Academy Trust",
            new Category { Id = "010", Name = "Multi Academy Trust" },
            establishmentNumber: null,
            localAuthority: null
        );

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
    /// Lets a builder be passed wherever an <see cref="Organisation"/> is expected, without
    /// calling <see cref="Build"/>.
    /// </summary>
    public static implicit operator Organisation(OrganisationBuilder builder) => builder.Build();

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
