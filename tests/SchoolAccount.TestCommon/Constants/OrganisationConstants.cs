namespace SchoolAccount.TestCommon.Constants;

public static class OrganisationConstants
{
    public const string FakeSchoolId = "8824EF2C-49B8-485D-8117-8767115E21F1";
    public const string FakeSchoolName = "Test School";
    public const string FakeCategoryEstablishmentId = "001";
    public const string FakeCategoryEstablishmentName = "Establishment";
    public const string FakeCategoryTrustId = "010";
    public const string FakeCategoryTrustName = "Multi Academy Trust";
    public const string FakeSchoolUkprn = "19999901";
    public const string FakeSchoolEstablishmentNumber = "2001";
    public const string FakeSchoolLocalAuthorityId = "00000000-0000-0000-0000-000000000999";
    public const string FakeSchoolLocalAuthorityName = "TEST COUNCIL";
    public const string FakeSchoolLocalAuthorityCode = "999";
    public const string FakeTrustId = "2D2CD02F-2CB1-4836-A390-C1D828EAEA4A";
    public const string FakeTrustName = "Test Multi Academy Trust";
    public const string FakeTrustUkprn = "19999902";
    public const string FakeTrustFirstEstablishmentUrn = "999901";
    public const string FakeTrustFirstEstablishmentName = "Test Establishment 1";
    public const string FakeTrustSecondEstablishmentUrn = "999902";
    public const string FakeTrustSecondEstablishmentName = "Test Establishment 2";

    public const string SingleSchoolJson = $$"""
        {
         "id": "{{FakeSchoolId}}",
         "name": "{{FakeSchoolName}}",
         "category": {
             "id": "{{FakeCategoryEstablishmentId}}",
             "name": "{{FakeCategoryEstablishmentName}}"
         },
         "ukprn": "{{FakeSchoolUkprn}}",
         "establishmentNumber": "{{FakeSchoolEstablishmentNumber}}",
         "localAuthority": {
             "id": "{{FakeSchoolLocalAuthorityId}}",
             "name": "{{FakeSchoolLocalAuthorityName}}",
             "code": "{{FakeSchoolLocalAuthorityCode}}"
         }
        }
        """;

    public const string TrustJson = $$"""
        {
         "id": "{{FakeTrustId}}",
         "name": "{{FakeTrustName}}",
         "category": { "id": "{{FakeCategoryTrustId}}", "name": "{{FakeCategoryTrustName}}" },
         "ukprn": "{{FakeTrustUkprn}}",
         "establishments": [
             { "urn": "{{FakeTrustFirstEstablishmentUrn}}", "name": "{{FakeTrustFirstEstablishmentName}}" },
             { "urn": "{{FakeTrustSecondEstablishmentUrn}}", "name": "{{FakeTrustSecondEstablishmentName}}" }
         ]
        }
        """;
}
