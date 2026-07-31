using static Microsoft.AspNetCore.Http.StatusCodes;

namespace SchoolAccount.Web.Mvc.Models;

public class ErrorViewModel(int statusCode)
{
    public int StatusCode { get; } =  statusCode;

    public string Title { get; } = string.Empty;

    public bool IsNotFound => StatusCode == Status404NotFound; 

    public bool IsForbidden => StatusCode == Status403Forbidden;
}
