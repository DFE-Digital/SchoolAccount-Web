namespace SchoolAccount.TestCommon.Constants;

public static class OrganisationConstants
{
    public const string FakeOrganisationId = "OOOOOOOO-OOOO-OOOO-OOOO-OOOOOOOOOOOO";
    public const string FakeOrganisationName = "Test School";
    public const string FakeCategoryEstablishmentId = "001";
    public const string FakeCategoryEstablishmentName = "Establishment";
    public const string FakeCategoryTrustId = "010";
    public const string FakeCategoryTrustName = "Multi Academy Trust";
    public const string FakeOrganisationUkprn = "12345678";
    public const string FakeAcademyEstablishmentNumber = "4567";
    public const string FakeAcademyLocalAuthorityId = "LALALALA-LALA-LALA-LALA-LALALALALALA";
    public const string FakeAcademyLocalAuthorityName = "TEST COUNCIL";
    public const string FakeAcademyLocalAuthorityCode = "123";
    public const string FakeTrustId = "TTTTTTTT-TTTT-TTTT-TTTT-TTTTTTTTTTTT";
    public const string FakeTrustName = "Test Multi Academy Trust";
    public const string FakeTrustUkprn = "87654321";
    public const string FakeTrustFirstEstablishmentUrn = "123456";
    public const string FakeTrustFirstEstablishmentName = "Test Establishment 1";
    public const string FakeTrustSecondEstablishmentUrn = "456789";
    public const string FakeTrustSecondEstablishmentName = "Test Establishment 2";

    public const string ValidAcademyJson = $$"""
        {
         "id": "{{FakeOrganisationId}}",
         "name": "{{FakeOrganisationName}}",
         "category": {
             "id": "{{FakeCategoryEstablishmentId}}",
             "name": "{{FakeCategoryEstablishmentName}}"
         },
         "ukprn": "{{FakeOrganisationUkprn}}",
         "establishmentNumber": "{{FakeAcademyEstablishmentNumber}}",
         "localAuthority": {
             "id": "{{FakeAcademyLocalAuthorityId}}",
             "name": "{{FakeAcademyLocalAuthorityName}}",
             "code": "{{FakeAcademyLocalAuthorityCode}}"
         }
        }
        """;

    public const string ValidTrustJson = $$"""
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
