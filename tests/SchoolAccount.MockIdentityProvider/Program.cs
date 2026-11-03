using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchoolAccount.MockIdentityProvider;

// A stand-in for DfE Sign-in, for running the app and its end-to-end tests
// locally or in a pipeline. It implements only what the web app uses: the
// implicit flow with response_type=id_token and response_mode=form_post.
// Every endpoint is anonymous by design. Never deploy this.
var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddOptions<MockIdentityProviderSettings>()
    .BindConfiguration(MockIdentityProviderSettings.SectionName)
    .ValidateOnStart();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IdTokenIssuer>();
builder.Services.AddSingleton(provider =>
{
    var settings = provider.GetRequiredService<IOptions<MockIdentityProviderSettings>>().Value;
    var path = Path.Combine(AppContext.BaseDirectory, settings.UsersFile);
    return TestUserStore.Load(path);
});

var app = builder.Build();

var settings = app.Services.GetRequiredService<IOptions<MockIdentityProviderSettings>>().Value;
var discoveryDocument = new Dictionary<string, object>
{
    ["issuer"] = settings.Issuer,
    // The browser follows these two, so they use the public address.
    ["authorization_endpoint"] = $"{settings.PublicUrl}/authorize",
    ["end_session_endpoint"] = $"{settings.PublicUrl}/endsession",
    // The web app fetches this itself, so it uses the issuer address.
    ["jwks_uri"] = $"{settings.Issuer}/jwks",
    ["response_types_supported"] = new[] { "id_token" },
    ["response_modes_supported"] = new[] { "form_post" },
    ["subject_types_supported"] = new[] { "public" },
    ["id_token_signing_alg_values_supported"] = new[] { SecurityAlgorithms.RsaSha256 },
    ["scopes_supported"] = new[] { "openid", "organisation", "email", "profile" },
};

app.MapGet("/.well-known/openid-configuration", () => Results.Json(discoveryDocument));

app.MapGet(
    "/jwks",
    (IdTokenIssuer issuer) => Results.Json(new { keys = new[] { issuer.PublicKey } })
);

app.MapGet(
    "/authorize",
    Results<ContentHttpResult, BadRequest<string>> (
        [FromQuery(Name = "client_id")] string clientId,
        [FromQuery(Name = "redirect_uri")] string redirectUri,
        [FromQuery(Name = "response_type")] string responseType,
        [FromQuery(Name = "response_mode")] string? responseMode,
        [FromQuery(Name = "state")] string? state,
        [FromQuery(Name = "nonce")] string nonce,
        IOptions<MockIdentityProviderSettings> options,
        TestUserStore users
    ) =>
    {
        if (!IsKnownClient(options.Value, clientId, redirectUri))
        {
            return TypedResults.BadRequest("Unknown client_id or redirect_uri.");
        }

        if (responseType != "id_token" || responseMode is not (null or "form_post"))
        {
            return TypedResults.BadRequest(
                "Only response_type=id_token with form_post is supported."
            );
        }

        var page = Pages.ChooseUser(users.Users, clientId, redirectUri, state ?? "", nonce);
        return TypedResults.Content(page, "text/html");
    }
);

app.MapPost(
        "/authorize",
        Results<ContentHttpResult, BadRequest<string>> (
            [FromForm(Name = "client_id")] string clientId,
            [FromForm(Name = "redirect_uri")] string redirectUri,
            [FromForm(Name = "state")] string state,
            [FromForm(Name = "nonce")] string nonce,
            [FromForm(Name = "user")] string userId,
            IOptions<MockIdentityProviderSettings> options,
            TestUserStore users,
            IdTokenIssuer issuer
        ) =>
        {
            if (!IsKnownClient(options.Value, clientId, redirectUri))
            {
                return TypedResults.BadRequest("Unknown client_id or redirect_uri.");
            }

            var user = users.Find(userId);
            if (user is null)
            {
                return TypedResults.BadRequest($"No test user with id '{userId}'.");
            }

            var idToken = issuer.Issue(user, clientId, nonce);
            return TypedResults.Content(Pages.FormPost(redirectUri, idToken, state), "text/html");
        }
    )
    // The form comes from this provider's own page, and nothing here is worth forging.
    .DisableAntiforgery();

app.MapGet(
    "/endsession",
    Results<RedirectHttpResult, Ok<string>> (
        [FromQuery(Name = "post_logout_redirect_uri")] string? postLogoutRedirectUri,
        [FromQuery(Name = "state")] string? state,
        IOptions<MockIdentityProviderSettings> options
    ) =>
    {
        if (
            postLogoutRedirectUri is null
            || !options.Value.AllowedRedirectUris.Contains(
                postLogoutRedirectUri,
                StringComparer.Ordinal
            )
        )
        {
            return TypedResults.Ok("Signed out.");
        }

        var target = state is null
            ? postLogoutRedirectUri
            : QueryHelpers.AddQueryString(postLogoutRedirectUri, "state", state);
        return TypedResults.Redirect(target);
    }
);

await app.RunAsync();

static bool IsKnownClient(
    MockIdentityProviderSettings settings,
    string clientId,
    string redirectUri
) =>
    string.Equals(clientId, settings.ClientId, StringComparison.Ordinal)
    && settings.AllowedRedirectUris.Contains(redirectUri, StringComparer.Ordinal);
