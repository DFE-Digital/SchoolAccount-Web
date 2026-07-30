using AngleSharp;
using AngleSharp.Dom;

namespace SchoolAccount.IntegrationTests.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task<IDocument?> GetPage(
        this HttpResponseMessage response,
        CancellationToken ct = default
    )
    {
        string html = await response.Content.ReadAsStringAsync(ct);
        IBrowsingContext context = BrowsingContext.New(Configuration.Default);
        return await context.OpenAsync(req => req.Content(html), ct);
    }
}
