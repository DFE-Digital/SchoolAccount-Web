using System.Text.Json.Serialization;

namespace SchoolAccount.Application.Features.Academies;

public class GetAcademyNameAndCodeResponse
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }
}
