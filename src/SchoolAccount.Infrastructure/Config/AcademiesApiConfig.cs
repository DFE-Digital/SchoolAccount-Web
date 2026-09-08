using System.ComponentModel.DataAnnotations;

namespace SchoolAccount.Infrastructure.Config;

public class AcademiesApiConfig
{
    public const string SectionName = "AcademiesApiSettings";

    [Required]
    public string BaseUrl { get; set; }

    [Required]
    public string ApiKey { get; set; }
}
