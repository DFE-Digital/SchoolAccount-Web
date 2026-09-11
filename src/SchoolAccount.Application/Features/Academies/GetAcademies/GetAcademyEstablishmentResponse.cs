namespace SchoolAccount.Application.Features.Academies.GetAcademies;

public class GetAcademyEstablishmentResponse
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
