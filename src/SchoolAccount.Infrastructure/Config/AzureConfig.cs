using System.ComponentModel.DataAnnotations;

namespace SchoolAccount.Infrastructure.Config;

public class AzureConfig
{
    public const string SectionName = "Azure";

    [Required]
    public string TableStorageConnectionString { get; set; }
}
