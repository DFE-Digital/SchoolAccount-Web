using System.Text.Json.Serialization;

namespace SchoolAccount.Application.Features.Academies;

public class GetAcademyEstablishmentResponse
{
    public string Urn { get; set; } = string.Empty;

    public string Ukprn { get; set; } = string.Empty;

    public string EstablishmentNumber { get; set; } = string.Empty;

    public string EstablishmentName { get; set; } = string.Empty;

    public string LocalAuthorityCode { get; set; } = string.Empty;

    public string LocalAuthorityName { get; set; } = string.Empty;

    public GetAcademyNameAndCodeResponse? EstablishmentType { get; set; }

    public GetAcademyNameAndCodeResponse? EstablishmentGroupType { get; set; }

    public GetAcademyNameAndCodeResponse? PhaseOfEducation { get; set; }
}
