namespace SchoolAccount.TestCommon.Constants;

public static class OrganisationConstants
{
    public const string ValidAcademyJson = """
        {
         "id": "2E774B32-E4DB-445B-B915-736C777FF5A4",
         "name": "Test School",
         "category": {
             "id": "001",
             "name": "Establishment"
         },
         "ukprn": "10037611",
         "establishmentNumber": "2091",
         "localAuthority": {
             "id": "502EF2E9-2CA6-4905-9BF7-E80695BD5717",
             "name": "SUNDERLAND CITY METROPOLITAN BOROUGH COUNCIL",
             "code": "394"
         }
        }
        """;
    public const string ValidTrustJson = """
        {
         "id": "2E774B32-E4DB-445B-B915-736C777FF5A4",
         "name": "Test Multi Academy Trust",
         "category": { "id": "010", "name": "Multi Academy Trust" },
         "ukprn": "10037611",
         "establishments": [
             { "urn": "100001", "name": "Test School 1" },
             { "urn": "100002", "name": "Test School 2" }
         ]
        }
        """;
}
