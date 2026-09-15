namespace SchoolAccount.Application.Features.Collect.GetCensusJourney.Responses;

public record GetAcademyTrustResponse
{
    public required string Name { get; init; }

    public required string Ukprn { get; init; }

    public GetAcademyNameAndCodeResponse? Type { get; init; }

    public string? GroupUid { get; init; }

    public IReadOnlyList<GetAcademyEstablishmentResponse>? Establishments { get; init; } = [];
}

public record GetAcademyEstablishmentResponse
{
    public required string Ukprn { get; init; }

    public required string EstablishmentName { get; init; }

    public string? Urn { get; init; }

    public string? EstablishmentNumber { get; init; }

    public string? LocalAuthorityCode { get; init; }

    public string? LocalAuthorityName { get; init; }

    public GetAcademyNameAndCodeResponse? EstablishmentType { get; init; }

    public GetAcademyNameAndCodeResponse? EstablishmentGroupType { get; init; }

    public GetAcademyNameAndCodeResponse? PhaseOfEducation { get; init; }
}

public record GetAcademyNameAndCodeResponse
{
    public required string Name { get; init; }

    public required string Code { get; init; }
}
