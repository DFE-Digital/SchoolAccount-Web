using System.Text.Json.Serialization;

namespace SchoolAccount.Application.Features.Academies;

public class AcademyEstablishment
{
    [JsonPropertyName("urn")]
    public string Urn { get; set; } = string.Empty;

    [JsonPropertyName("localAuthorityCode")]
    public string LocalAuthorityCode { get; set; } = string.Empty;

    [JsonPropertyName("localAuthorityName")]
    public string LocalAuthorityName { get; set; } = string.Empty;

    [JsonPropertyName("establishmentNumber")]
    public string EstablishmentNumber { get; set; } = string.Empty;

    [JsonPropertyName("establishmentName")]
    public string EstablishmentName { get; set; } = string.Empty;

    [JsonPropertyName("establishmentType")]
    public NameAndCode? EstablishmentType { get; set; }

    [JsonPropertyName("establishmentTypeGroup")]
    public NameAndCode? EstablishmentTypeGroup { get; set; }

    [JsonPropertyName("establishmentStatus")]
    public NameAndCode? EstablishmentStatus { get; set; }

    [JsonPropertyName("reasonEstablishmentOpened")]
    public NameAndCode? ReasonEstablishmentOpened { get; set; }

    [JsonPropertyName("openDate")]
    public string? OpenDate { get; set; }

    [JsonPropertyName("reasonEstablishmentClosed")]
    public NameAndCode? ReasonEstablishmentClosed { get; set; }

    [JsonPropertyName("closeDate")]
    public string? CloseDate { get; set; }

    [JsonPropertyName("phaseOfEducation")]
    public NameAndCode? PhaseOfEducation { get; set; }

    [JsonPropertyName("statutoryLowAge")]
    public string StatutoryLowAge { get; set; } = string.Empty;

    [JsonPropertyName("statutoryHighAge")]
    public string StatutoryHighAge { get; set; } = string.Empty;

    [JsonPropertyName("boarders")]
    public NameAndCode? Boarders { get; set; }

    [JsonPropertyName("nurseryProvision")]
    public string NurseryProvision { get; set; } = string.Empty;

    [JsonPropertyName("officialSixthForm")]
    public NameAndCode? OfficialSixthForm { get; set; }

    [JsonPropertyName("gender")]
    public NameAndCode? Gender { get; set; }

    [JsonPropertyName("religiousCharacter")]
    public NameAndCode? ReligiousCharacter { get; set; }

    [JsonPropertyName("religiousEthos")]
    public string ReligiousEthos { get; set; } = string.Empty;

    [JsonPropertyName("diocese")]
    public NameAndCode? Diocese { get; set; }

    [JsonPropertyName("admissionsPolicy")]
    public NameAndCode? AdmissionsPolicy { get; set; }

    [JsonPropertyName("schoolCapacity")]
    public string SchoolCapacity { get; set; } = string.Empty;

    [JsonPropertyName("specialClasses")]
    public NameAndCode? SpecialClasses { get; set; }

    [JsonPropertyName("census")]
    public CensusData? Census { get; set; }

    [JsonPropertyName("trustSchoolFlag")]
    public NameAndCode? TrustSchoolFlag { get; set; }

    [JsonPropertyName("trusts")]
    public NameAndCode? Trusts { get; set; }

    [JsonPropertyName("schoolSponsorFlag")]
    public string SchoolSponsorFlag { get; set; } = string.Empty;

    [JsonPropertyName("schoolSponsors")]
    public string? SchoolSponsors { get; set; }

    [JsonPropertyName("federationFlag")]
    public string FederationFlag { get; set; } = string.Empty;

    [JsonPropertyName("federations")]
    public NameAndCode? Federations { get; set; }

    [JsonPropertyName("ukprn")]
    public string Ukprn { get; set; } = string.Empty;

    [JsonPropertyName("feheiIdentifier")]
    public string? FeheiIdentifier { get; set; }

    [JsonPropertyName("furtherEducationType")]
    public string FurtherEducationType { get; set; } = string.Empty;

    [JsonPropertyName("ofstedLastInspection")]
    public string OfstedLastInspection { get; set; } = string.Empty;

    [JsonPropertyName("ofstedSpecialMeasures")]
    public NameAndCode? OfstedSpecialMeasures { get; set; }

    [JsonPropertyName("lastChangedDate")]
    public string LastChangedDate { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public AddressData? Address { get; set; }

    [JsonPropertyName("schoolWebsite")]
    public string SchoolWebsite { get; set; } = string.Empty;

    [JsonPropertyName("telephoneNumber")]
    public string TelephoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("headteacherTitle")]
    public string HeadteacherTitle { get; set; } = string.Empty;

    [JsonPropertyName("headteacherFirstName")]
    public string HeadteacherFirstName { get; set; } = string.Empty;

    [JsonPropertyName("headteacherLastName")]
    public string HeadteacherLastName { get; set; } = string.Empty;

    [JsonPropertyName("headteacherPreferredJobTitle")]
    public string HeadteacherPreferredJobTitle { get; set; } = string.Empty;

    [JsonPropertyName("inspectorateName")]
    public string? InspectorateName { get; set; }

    [JsonPropertyName("inspectorateReport")]
    public string? InspectorateReport { get; set; }

    [JsonPropertyName("dateOfLastInspectionVisit")]
    public string? DateOfLastInspectionVisit { get; set; }

    [JsonPropertyName("dateOfNextInspectionVisit")]
    public string? DateOfNextInspectionVisit { get; set; }

    [JsonPropertyName("teenMoth")]
    public string TeenMoth { get; set; } = string.Empty;

    [JsonPropertyName("teenMothPlaces")]
    public string? TeenMothPlaces { get; set; }

    [JsonPropertyName("ccf")]
    public string Ccf { get; set; } = string.Empty;

    [JsonPropertyName("senpru")]
    public string Senpru { get; set; } = string.Empty;

    [JsonPropertyName("ebd")]
    public string Ebd { get; set; } = string.Empty;

    [JsonPropertyName("placesPRU")]
    public string? PlacesPru { get; set; }

    [JsonPropertyName("ftProv")]
    public string? FtProv { get; set; }

    [JsonPropertyName("edByOther")]
    public string EdByOther { get; set; } = string.Empty;

    [JsonPropertyName("section14Approved")]
    public string Section14Approved { get; set; } = string.Empty;

    [JsonPropertyName("seN1")]
    public string? SeN1 { get; set; }

    [JsonPropertyName("seN2")]
    public string? SeN2 { get; set; }

    [JsonPropertyName("seN3")]
    public string? SeN3 { get; set; }

    [JsonPropertyName("seN4")]
    public string? SeN4 { get; set; }

    [JsonPropertyName("seN5")]
    public string? SeN5 { get; set; }

    [JsonPropertyName("seN6")]
    public string? SeN6 { get; set; }

    [JsonPropertyName("seN7")]
    public string? SeN7 { get; set; }

    [JsonPropertyName("seN8")]
    public string? SeN8 { get; set; }

    [JsonPropertyName("seN9")]
    public string? SeN9 { get; set; }

    [JsonPropertyName("seN10")]
    public string? SeN10 { get; set; }

    [JsonPropertyName("seN11")]
    public string? SeN11 { get; set; }

    [JsonPropertyName("seN12")]
    public string? SeN12 { get; set; }

    [JsonPropertyName("seN13")]
    public string? SeN13 { get; set; }

    [JsonPropertyName("typeOfResourcedProvision")]
    public string? TypeOfResourcedProvision { get; set; }

    [JsonPropertyName("resourcedProvisionOnRoll")]
    public string? ResourcedProvisionOnRoll { get; set; }

    [JsonPropertyName("resourcedProvisionOnCapacity")]
    public string? ResourcedProvisionOnCapacity { get; set; }

    [JsonPropertyName("senUnitOnRoll")]
    public string? SenUnitOnRoll { get; set; }

    [JsonPropertyName("senUnitCapacity")]
    public string? SenUnitCapacity { get; set; }

    [JsonPropertyName("gor")]
    public NameAndCode? Gor { get; set; }

    [JsonPropertyName("districtAdministrative")]
    public NameAndCode? DistrictAdministrative { get; set; }

    [JsonPropertyName("administractiveWard")]
    public NameAndCode? AdministractiveWard { get; set; }

    [JsonPropertyName("parliamentaryConstituency")]
    public NameAndCode? ParliamentaryConstituency { get; set; }

    [JsonPropertyName("urbanRural")]
    public NameAndCode? UrbanRural { get; set; }

    [JsonPropertyName("gsslaCode")]
    public string GsslaCode { get; set; } = string.Empty;

    [JsonPropertyName("easting")]
    public string Easting { get; set; } = string.Empty;

    [JsonPropertyName("northing")]
    public string Northing { get; set; } = string.Empty;

    [JsonPropertyName("censusAreaStatisticWard")]
    public string? CensusAreaStatisticWard { get; set; }

    [JsonPropertyName("msoa")]
    public NameAndCode? Msoa { get; set; }

    [JsonPropertyName("lsoa")]
    public NameAndCode? Lsoa { get; set; }

    [JsonPropertyName("senStat")]
    public string? SenStat { get; set; }

    [JsonPropertyName("senNoStat")]
    public string? SenNoStat { get; set; }

    [JsonPropertyName("boardingEstablishment")]
    public string? BoardingEstablishment { get; set; }

    [JsonPropertyName("propsName")]
    public string? PropsName { get; set; }

    [JsonPropertyName("previousLocalAuthority")]
    public NameAndCode? PreviousLocalAuthority { get; set; }

    [JsonPropertyName("previousEstablishmentNumber")]
    public string? PreviousEstablishmentNumber { get; set; }

    [JsonPropertyName("ofstedRating")]
    public string OfstedRating { get; set; } = string.Empty;

    [JsonPropertyName("rscRegion")]
    public string? RscRegion { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("uprn")]
    public string Uprn { get; set; } = string.Empty;

    [JsonPropertyName("misEstablishment")]
    public MisEstablishmentData? MisEstablishment { get; set; }

    [JsonPropertyName("misFurtherEducationEstablishment")]
    public object? MisFurtherEducationEstablishment { get; set; }

    [JsonPropertyName("viewAcademyConversion")]
    public ViewAcademyConversionData? ViewAcademyConversion { get; set; }

    [JsonPropertyName("smartData")]
    public SmartData? SmartData { get; set; }

    [JsonPropertyName("financial")]
    public object? Financial { get; set; }

    [JsonPropertyName("concerns")]
    public object? Concerns { get; set; }
}

public class ViewAcademyConversionData
{
    [JsonPropertyName("viabilityIssue")]
    public string? ViabilityIssue { get; set; }

    [JsonPropertyName("pfi")]
    public string? Pfi { get; set; }

    [JsonPropertyName("pan")]
    public string? Pan { get; set; }

    [JsonPropertyName("deficit")]
    public string? Deficit { get; set; }
}
