using System.ComponentModel.DataAnnotations;

namespace SchoolAccount.Infrastructure.Config;

public class CommonApiConfig
{
    public const string SectionName = "CommonApiSettings";

    [Required]
    public string CollectApiUrl { get; set; }
}
