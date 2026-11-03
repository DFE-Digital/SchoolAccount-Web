using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;

namespace SchoolAccount.MockIdentityProvider;

internal static class Pages
{
    private static readonly HtmlEncoder _html = HtmlEncoder.Default;

    /// <summary>
    /// Stands in for DfE Sign-in's login and choose-organisation steps: one
    /// button per test user, so a test signs in with a single click.
    /// </summary>
    public static string ChooseUser(
        IEnumerable<TestUser> users,
        string clientId,
        string redirectUri,
        string state,
        string nonce
    )
    {
        var forms = new StringBuilder();
        foreach (var user in users)
        {
            forms.Append(
                CultureInfo.InvariantCulture,
                $"""
                <form method="post" action="/authorize">
                  {Hidden("client_id", clientId)}
                  {Hidden("redirect_uri", redirectUri)}
                  {Hidden("state", state)}
                  {Hidden("nonce", nonce)}
                  {Hidden("user", user.Id)}
                  <button type="submit" id="{_html.Encode(user.Id)}">Sign in as {_html.Encode(
                    user.OrganisationName
                )}</button>
                  <span>{_html.Encode(user.Email)}</span>
                </form>
                """
            );
        }

        return Document("Mock identity provider", $"<h1>Choose a test user</h1>{forms}");
    }

    /// <summary>
    /// Posts the ID token back to the web app, as the <c>form_post</c>
    /// response mode requires.
    /// </summary>
    public static string FormPost(string redirectUri, string idToken, string state) =>
        Document(
            "Signing in",
            $"""
            <form method="post" action="{_html.Encode(redirectUri)}">
              {Hidden("id_token", idToken)}
              {Hidden("state", state)}
              <noscript><button type="submit">Continue</button></noscript>
            </form>
            <script>document.forms[0].submit();</script>
            """
        );

    private static string Hidden(string name, string value) =>
        $"""<input type="hidden" name="{name}" value="{_html.Encode(value)}">""";

    private static string Document(string title, string body) =>
        $"""
            <!doctype html>
            <html lang="en">
            <head><meta charset="utf-8"><title>{title}</title></head>
            <body>{body}</body>
            </html>
            """;
}
