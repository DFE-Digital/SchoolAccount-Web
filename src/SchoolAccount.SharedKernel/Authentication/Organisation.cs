namespace SchoolAccount.SharedKernel.Authentication;

public record Organisation
{
    public const string EstablishmentCategoryId = "001";

    private const string _matCategoryId = "010";

    public string Id { get; init; }

    public string Name { get; init; }

    public Category Category { get; init; }

    public string? Ukprn { get; init; }

    public LocalAuthority? LocalAuthority { get; init; }

    public string? EstablishmentNumber { get; init; }

    public bool IsEstablishment =>
        Category.Id.Equals(EstablishmentCategoryId, StringComparison.OrdinalIgnoreCase);

    public bool IsMat => Category.Id.Equals(_matCategoryId, StringComparison.OrdinalIgnoreCase);
}

public record Category
{
    public string Id { get; init; }

    public string Name { get; init; }
}

public record LocalAuthority
{
    public string Id { get; init; }

    public string Name { get; init; }

    public string Code { get; init; }
}
