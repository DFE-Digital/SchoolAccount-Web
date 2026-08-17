using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using NSubstitute;

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
                claims.Add(new Claim("given_name", givenName));
            }

            if (surname != null)
            {
                claims.Add(new Claim("family_name", surname));
            }

            if (email != null)
            {
                claims.Add(new Claim("email", email));
                claims.Add(new Claim("sid", Convert.ToBase64String(Encoding.UTF8.GetBytes(email))));
            }

            if (organisationName != null)
            {
                claims.Add(new Claim("org_name", organisationName));
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
