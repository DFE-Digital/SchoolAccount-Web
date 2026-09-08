namespace SchoolAccount.Application.Features.Academies.GetAcademies;

public class GetAcademyEstablishmentResponse
{
    public required string Ukprn { get; init; } = string.Empty;

    public required string EstablishmentName { get; init; } = string.Empty;

    public string Urn { get; init; } = string.Empty;

    public string EstablishmentNumber { get; init; } = string.Empty;

    public string LocalAuthorityCode { get; init; } = string.Empty;

    public string LocalAuthorityName { get; init; } = string.Empty;

    public GetAcademyNameAndCodeResponse? EstablishmentType { get; init; }

    public GetAcademyNameAndCodeResponse? EstablishmentGroupType { get; init; }

    public GetAcademyNameAndCodeResponse? PhaseOfEducation { get; init; }
}
