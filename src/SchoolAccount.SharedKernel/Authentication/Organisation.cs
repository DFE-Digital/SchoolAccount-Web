namespace SchoolAccount.SharedKernel.Authentication;

public record Organisation
{
    public string Id { get; init; }

    public string Name { get; init; }

    public Category Category { get; init; }

    public string Ukprn { get; init; }

    public LocalAuthority? LocalAuthority { get; init; }

    public string? EstablishmentNumber { get; init; }
}

public record Category
{
    public string Id { get; init; }

    public string Name { get; init; }

    public OrganisationCategory Value =>
        int.TryParse(Id, out var id) && Enum.IsDefined(typeof(OrganisationCategory), id)
            ? (OrganisationCategory)id
            : OrganisationCategory.Unknown;
}

public record LocalAuthority
{
    public string Id { get; init; }

    public string Name { get; init; }

    public string Code { get; init; }
}
