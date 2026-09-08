using System.Text.Json.Serialization;

namespace SchoolAccount.Application.Features.Academies;

public class AcademyOrganisation
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
    public object? ViewAcademyConversion { get; set; }

    [JsonPropertyName("smartData")]
    public SmartData? SmartData { get; set; }

    [JsonPropertyName("financial")]
    public object? Financial { get; set; }

    [JsonPropertyName("concerns")]
    public object? Concerns { get; set; }
}

public class NameAndCode
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }
}

public class CensusData
{
    [JsonPropertyName("censusDate")]
    public string CensusDate { get; set; } = string.Empty;

    [JsonPropertyName("numberOfPupils")]
    public string NumberOfPupils { get; set; } = string.Empty;

    [JsonPropertyName("numberOfBoys")]
    public string NumberOfBoys { get; set; } = string.Empty;

    [JsonPropertyName("numberOfGirls")]
    public string NumberOfGirls { get; set; } = string.Empty;

    [JsonPropertyName("percentageSen")]
    public string PercentageSen { get; set; } = string.Empty;

    [JsonPropertyName("percentageFsm")]
    public string PercentageFsm { get; set; } = string.Empty;

    [JsonPropertyName("percentageEnglishNotFirstLanguage")]
    public string PercentageEnglishNotFirstLanguage { get; set; } = string.Empty;

    [JsonPropertyName("percentageEnglishFirstLanguage")]
    public string PercentageEnglishFirstLanguage { get; set; } = string.Empty;

    [JsonPropertyName("percentageFirstLanguageUnclassified")]
    public string PercentageFirstLanguageUnclassified { get; set; } = string.Empty;

    [JsonPropertyName("numberEligibleForFSM")]
    public string NumberEligibleForFsm { get; set; } = string.Empty;

    [JsonPropertyName("numberEligibleForFSM6Years")]
    public string NumberEligibleForFsm6Years { get; set; } = string.Empty;

    [JsonPropertyName("percentageEligibleForFSM6Years")]
    public string PercentageEligibleForFsm6Years { get; set; } = string.Empty;
}

public class AddressData
{
    [JsonPropertyName("street")]
    public string Street { get; set; } = string.Empty;

    [JsonPropertyName("locality")]
    public string Locality { get; set; } = string.Empty;

    [JsonPropertyName("additionalLine")]
    public string? AdditionalLine { get; set; }

    [JsonPropertyName("town")]
    public string Town { get; set; } = string.Empty;

    [JsonPropertyName("county")]
    public string? County { get; set; }

    [JsonPropertyName("postcode")]
    public string Postcode { get; set; } = string.Empty;
}

public class MisEstablishmentData
{
    [JsonPropertyName("siteName")]
    public string? SiteName { get; set; }

    [JsonPropertyName("webLink")]
    public string WebLink { get; set; } = string.Empty;

    [JsonPropertyName("laestab")]
    public string Laestab { get; set; } = string.Empty;

    [JsonPropertyName("schoolName")]
    public string SchoolName { get; set; } = string.Empty;

    [JsonPropertyName("ofstedPhase")]
    public string OfstedPhase { get; set; } = string.Empty;

    [JsonPropertyName("typeOfEducation")]
    public string TypeOfEducation { get; set; } = string.Empty;

    [JsonPropertyName("schoolOpenDate")]
    public string? SchoolOpenDate { get; set; }

    [JsonPropertyName("sixthForm")]
    public string SixthForm { get; set; } = string.Empty;

    [JsonPropertyName("designatedReligiousCharacter")]
    public string DesignatedReligiousCharacter { get; set; } = string.Empty;

    [JsonPropertyName("religiousEthos")]
    public string ReligiousEthos { get; set; } = string.Empty;

    [JsonPropertyName("faithGrouping")]
    public string FaithGrouping { get; set; } = string.Empty;

    [JsonPropertyName("ofstedRegion")]
    public string OfstedRegion { get; set; } = string.Empty;

    [JsonPropertyName("region")]
    public string Region { get; set; } = string.Empty;

    [JsonPropertyName("localAuthority")]
    public string LocalAuthority { get; set; } = string.Empty;

    [JsonPropertyName("parliamentaryConstituency")]
    public string ParliamentaryConstituency { get; set; } = string.Empty;

    [JsonPropertyName("postcode")]
    public string Postcode { get; set; } = string.Empty;

    [JsonPropertyName("incomeDeprivationAffectingChildrenIndexQuintile")]
    public string IncomeDeprivationAffectingChildrenIndexQuintile { get; set; } = string.Empty;

    [JsonPropertyName("totalNumberOfPupils")]
    public string TotalNumberOfPupils { get; set; } = string.Empty;

    [JsonPropertyName("latestSection8InspectionNumberSinceLastFullInspection")]
    public string? LatestSection8InspectionNumberSinceLastFullInspection { get; set; }

    [JsonPropertyName("section8InspectionRelatedToCurrentSchoolUrn")]
    public string? Section8InspectionRelatedToCurrentSchoolUrn { get; set; }

    [JsonPropertyName("urnAtTimeOfSection8Inspection")]
    public string UrnAtTimeOfSection8Inspection { get; set; } = string.Empty;

    [JsonPropertyName("schoolNameAtTimeOfSection8Inspection")]
    public string? SchoolNameAtTimeOfSection8Inspection { get; set; }

    [JsonPropertyName("schoolTypeAtTimeOfSection8Inspection")]
    public string? SchoolTypeAtTimeOfSection8Inspection { get; set; }

    [JsonPropertyName("numberOfSection8InspectionsSinceLastFullInspection")]
    public string NumberOfSection8InspectionsSinceLastFullInspection { get; set; } = string.Empty;

    [JsonPropertyName("dateOfLatestSection8Inspection")]
    public string? DateOfLatestSection8Inspection { get; set; }

    [JsonPropertyName("section8InspectionPublicationDate")]
    public string? Section8InspectionPublicationDate { get; set; }

    [JsonPropertyName("latestSection8InspectionConvertedToFullInspection")]
    public string? LatestSection8InspectionConvertedToFullInspection { get; set; }

    [JsonPropertyName("section8InspectionOverallOutcome")]
    public string? Section8InspectionOverallOutcome { get; set; }

    [JsonPropertyName("inspectionNumberOfLatestFullInspection")]
    public string InspectionNumberOfLatestFullInspection { get; set; } = string.Empty;

    [JsonPropertyName("inspectionType")]
    public string InspectionType { get; set; } = string.Empty;

    [JsonPropertyName("inspectionTypeGrouping")]
    public string InspectionTypeGrouping { get; set; } = string.Empty;

    [JsonPropertyName("inspectionStartDate")]
    public string InspectionStartDate { get; set; } = string.Empty;

    [JsonPropertyName("inspectionEndDate")]
    public string? InspectionEndDate { get; set; }

    [JsonPropertyName("publicationDate")]
    public string PublicationDate { get; set; } = string.Empty;

    [JsonPropertyName("latestFullInspectionRelatesToCurrentSchoolUrn")]
    public string LatestFullInspectionRelatesToCurrentSchoolUrn { get; set; } = string.Empty;

    [JsonPropertyName("schoolUrnAtTimeOfLastFullInspection")]
    public string SchoolUrnAtTimeOfLastFullInspection { get; set; } = string.Empty;

    [JsonPropertyName("laestabAtTimeOfLastFullInspection")]
    public string LaestabAtTimeOfLastFullInspection { get; set; } = string.Empty;

    [JsonPropertyName("schoolNameAtTimeOfLastFullInspection")]
    public string SchoolNameAtTimeOfLastFullInspection { get; set; } = string.Empty;

    [JsonPropertyName("schoolTypeAtTimeOfLastFullInspection")]
    public string SchoolTypeAtTimeOfLastFullInspection { get; set; } = string.Empty;

    [JsonPropertyName("overallEffectiveness")]
    public string OverallEffectiveness { get; set; } = string.Empty;

    [JsonPropertyName("categoryOfConcern")]
    public string? CategoryOfConcern { get; set; }

    [JsonPropertyName("qualityOfEducation")]
    public string QualityOfEducation { get; set; } = string.Empty;

    [JsonPropertyName("behaviourAndAttitudes")]
    public string BehaviourAndAttitudes { get; set; } = string.Empty;

    [JsonPropertyName("personalDevelopment")]
    public string PersonalDevelopment { get; set; } = string.Empty;

    [JsonPropertyName("effectivenessOfLeadershipAndManagement")]
    public string EffectivenessOfLeadershipAndManagement { get; set; } = string.Empty;

    [JsonPropertyName("safeguardingIsEffective")]
    public string SafeguardingIsEffective { get; set; } = string.Empty;

    [JsonPropertyName("earlyYearsProvision")]
    public string EarlyYearsProvision { get; set; } = string.Empty;

    [JsonPropertyName("sixthFormProvision")]
    public string SixthFormProvision { get; set; } = string.Empty;

    [JsonPropertyName("previousFullInspectionNumber")]
    public string PreviousFullInspectionNumber { get; set; } = string.Empty;

    [JsonPropertyName("previousInspectionStartDate")]
    public string PreviousInspectionStartDate { get; set; } = string.Empty;

    [JsonPropertyName("previousInspectionEndDate")]
    public string? PreviousInspectionEndDate { get; set; }

    [JsonPropertyName("previousPublicationDate")]
    public string PreviousPublicationDate { get; set; } = string.Empty;

    [JsonPropertyName("previousFullInspectionRelatesToUrnOfCurrentSchool")]
    public string PreviousFullInspectionRelatesToUrnOfCurrentSchool { get; set; } = string.Empty;

    [JsonPropertyName("urnAtTheTimeOfPreviousFullInspection")]
    public string UrnAtTheTimeOfPreviousFullInspection { get; set; } = string.Empty;

    [JsonPropertyName("laestabAtTheTimeOfPreviousFullInspection")]
    public string LaestabAtTheTimeOfPreviousFullInspection { get; set; } = string.Empty;

    [JsonPropertyName("schoolNameAtTheTimeOfPreviousFullInspection")]
    public string SchoolNameAtTheTimeOfPreviousFullInspection { get; set; } = string.Empty;

    [JsonPropertyName("schoolTypeAtTheTimeOfPreviousFullInspection")]
    public string SchoolTypeAtTheTimeOfPreviousFullInspection { get; set; } = string.Empty;

    [JsonPropertyName("previousFullInspectionOverallEffectiveness")]
    public string PreviousFullInspectionOverallEffectiveness { get; set; } = string.Empty;

    [JsonPropertyName("previousCategoryOfConcern")]
    public string? PreviousCategoryOfConcern { get; set; }

    [JsonPropertyName("previousQualityOfEducation")]
    public string PreviousQualityOfEducation { get; set; } = string.Empty;

    [JsonPropertyName("previousBehaviourAndAttitudes")]
    public string PreviousBehaviourAndAttitudes { get; set; } = string.Empty;

    [JsonPropertyName("previousPersonalDevelopment")]
    public string PreviousPersonalDevelopment { get; set; } = string.Empty;

    [JsonPropertyName("previousEffectivenessOfLeadershipAndManagement")]
    public string PreviousEffectivenessOfLeadershipAndManagement { get; set; } = string.Empty;

    [JsonPropertyName("previousIsSafeguardingEffective")]
    public string PreviousIsSafeguardingEffective { get; set; } = string.Empty;

    [JsonPropertyName("previousEarlyYearsProvision")]
    public string PreviousEarlyYearsProvision { get; set; } = string.Empty;

    [JsonPropertyName("previousSixthFormProvision")]
    public string PreviousSixthFormProvision { get; set; } = string.Empty;
}

public class SmartData
{
    [JsonPropertyName("probabilityOfDeclining")]
    public string ProbabilityOfDeclining { get; set; } = string.Empty;

    [JsonPropertyName("probabilityOfStayingTheSame")]
    public string ProbabilityOfStayingTheSame { get; set; } = string.Empty;

    [JsonPropertyName("probabilityOfImproving")]
    public string ProbabilityOfImproving { get; set; } = string.Empty;

    [JsonPropertyName("predictedChangeInProgress8Score")]
    public string PredictedChangeInProgress8Score { get; set; } = string.Empty;

    [JsonPropertyName("predictedChanceOfChangeOccurring")]
    public string PredictedChanceOfChangeOccurring { get; set; } = string.Empty;

    [JsonPropertyName("totalNumberOfRisks")]
    public string TotalNumberOfRisks { get; set; } = string.Empty;

    [JsonPropertyName("totalRiskScore")]
    public string TotalRiskScore { get; set; } = string.Empty;

    [JsonPropertyName("riskRatingNum")]
    public string RiskRatingNum { get; set; } = string.Empty;
}
