using System.Text.Json.Serialization;

namespace SchoolAccount.Application.Features.Academies;

public class AcademyTrust
{
    [JsonPropertyName("ifdData")]
    public TrustIfdData? IfdData { get; set; }

    [JsonPropertyName("giasData")]
    public TrustGiasData? GiasData { get; set; }

    public List<AcademyEstablishment> Establishments { get; set; } = new();
}

public class TrustIfdData
{
    [JsonPropertyName("trustOpenDate")]
    public string? TrustOpenDate { get; set; }

    [JsonPropertyName("leadRSCRegion")]
    public string? LeadRscRegion { get; set; }

    [JsonPropertyName("trustContactPhoneNumber")]
    public string? TrustContactPhoneNumber { get; set; }

    [JsonPropertyName("performanceAndRiskDateOfMeeting")]
    public string? PerformanceAndRiskDateOfMeeting { get; set; }

    [JsonPropertyName("prioritisedAreaOfReview")]
    public string? PrioritisedAreaOfReview { get; set; }

    [JsonPropertyName("currentSingleListGrouping")]
    public string? CurrentSingleListGrouping { get; set; }

    [JsonPropertyName("dateOfGroupingDecision")]
    public string? DateOfGroupingDecision { get; set; }

    [JsonPropertyName("dateEnteredOntoSingleList")]
    public string? DateEnteredOntoSingleList { get; set; }

    [JsonPropertyName("trustReviewWriteup")]
    public string? TrustReviewWriteup { get; set; }

    [JsonPropertyName("dateOfTrustReviewMeeting")]
    public string? DateOfTrustReviewMeeting { get; set; }

    [JsonPropertyName("followupLetterSent")]
    public string? FollowupLetterSent { get; set; }

    [JsonPropertyName("dateActionPlannedFor")]
    public string? DateActionPlannedFor { get; set; }

    [JsonPropertyName("wipSummaryGoesToMinister")]
    public string? WipSummaryGoesToMinister { get; set; }

    [JsonPropertyName("externalGovernanceReviewDate")]
    public string? ExternalGovernanceReviewDate { get; set; }

    [JsonPropertyName("efficiencyICFPreviewCompleted")]
    public string? EfficiencyIcfPreviewCompleted { get; set; }

    [JsonPropertyName("efficiencyICFPreviewOther")]
    public string? EfficiencyIcfPreviewOther { get; set; }

    [JsonPropertyName("linkToWorkplaceForEfficiencyICFReview")]
    public string? LinkToWorkplaceForEfficiencyIcfReview { get; set; }

    [JsonPropertyName("numberInTrust")]
    public string? NumberInTrust { get; set; }

    [JsonPropertyName("trustType")]
    public string? TrustType { get; set; }

    [JsonPropertyName("trustAddress")]
    public AddressData? TrustAddress { get; set; }
}

public class TrustGiasData
{
    [JsonPropertyName("groupId")]
    public string? GroupId { get; set; }

    [JsonPropertyName("groupName")]
    public string? GroupName { get; set; }

    [JsonPropertyName("groupType")]
    public string? GroupType { get; set; }

    [JsonPropertyName("companiesHouseNumber")]
    public string? CompaniesHouseNumber { get; set; }

    [JsonPropertyName("groupContactAddress")]
    public AddressData? GroupContactAddress { get; set; }

    [JsonPropertyName("ukprn")]
    public string? Ukprn { get; set; }
}
