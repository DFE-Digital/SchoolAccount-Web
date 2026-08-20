using System.ComponentModel.DataAnnotations;

namespace SchoolAccount.Web.Mvc.Config;

public class CommonApiConfig
{
    [Required]
    public string CollectApiUrl { get; set; }
}
