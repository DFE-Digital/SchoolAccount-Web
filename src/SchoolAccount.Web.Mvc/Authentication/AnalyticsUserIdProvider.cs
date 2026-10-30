using System.Security.Cryptography;
using System.Text;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Authentication;

public sealed class AnalyticsUserIdProvider(IUserContext userContext, IConfiguration configuration)
{
    public string? GetUserId()
    {
        if (!userContext.IsAuthenticated || string.IsNullOrWhiteSpace(userContext.Id))
        {
            return null;
        }

        var key = configuration["CensusAnalytics:UserIdHashKey"];

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                "Census analytics user ID hashing key is not configured."
            );
        }

        var hash = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(key),
            Encoding.UTF8.GetBytes(userContext.Id)
        );

        return Convert.ToHexString(hash);
    }
}
