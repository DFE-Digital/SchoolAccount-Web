using System.Text.Json.Serialization;

namespace SchoolAccount.Application.Features.Academies;

public class GetAcademyOrganisationResponse
{
    [JsonPropertyName("urn")]
    public string Urn { get; set; } = string.Empty;

    [JsonPropertyName("uprn")]
    public string Uprn { get; set; } = string.Empty;

    [JsonPropertyName("ukprn")]
    public string Ukprn { get; set; } = string.Empty;

    [JsonPropertyName("localAuthorityCode")]
    public string LocalAuthorityCode { get; set; } = string.Empty;

    [JsonPropertyName("localAuthorityName")]
    public string LocalAuthorityName { get; set; } = string.Empty;

    [JsonPropertyName("establishmentNumber")]
    public string EstablishmentNumber { get; set; } = string.Empty;

    [JsonPropertyName("establishmentName")]
    public string EstablishmentName { get; set; } = string.Empty;

    [JsonPropertyName("establishmentType")]
    public GetAcademyNameAndCodeResponse? EstablishmentType { get; set; }

    [JsonPropertyName("establishmentTypeGroup")]
    public GetAcademyNameAndCodeResponse? EstablishmentTypeGroup { get; set; }

    [JsonPropertyName("phaseOfEducation")]
    public GetAcademyNameAndCodeResponse? PhaseOfEducation { get; set; }

    [JsonPropertyName("trustSchoolFlag")]
    public GetAcademyNameAndCodeResponse? TrustSchoolFlag { get; set; }

    [JsonPropertyName("trusts")]
    public GetAcademyNameAndCodeResponse? Trusts { get; set; }
}
