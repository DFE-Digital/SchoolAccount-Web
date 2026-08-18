using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using static SchoolAccount.Web.Mvc.Authentication.ClaimConstants;

namespace SchoolAccount.Web.Mvc.UnitTests.Helpers;

public static class HttpContextAccessorHelpers
{
    public static IHttpContextAccessor CreateHttpContextAccessor(
        bool isAuthenticated,
        string? givenName = null,
        string? surname = null,
        string? email = null,
        string? organisationName = null
    )
    {
        var claims = new List<Claim>();

        if (isAuthenticated)
        {
            if (givenName != null)
            {
                claims.Add(new Claim(GivenName, givenName));
            }

            if (surname != null)
            {
                claims.Add(new Claim(FamilyName, surname));
            }

            if (email != null)
            {
                claims.Add(new Claim(Email, email));
                claims.Add(new Claim(Id, Convert.ToBase64String(Encoding.UTF8.GetBytes(email))));
            }

            if (organisationName != null)
            {
                claims.Add(
                    new Claim(
                        Organisation,
                        JsonSerializer.Serialize(new { name = organisationName })
                    )
                );
            }
        }

        var identity = new ClaimsIdentity(claims, isAuthenticated ? "test" : null);
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = principal };

        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(httpContext);

        return accessor;
    }
}
