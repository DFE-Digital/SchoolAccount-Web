using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Html;

namespace SchoolAccount.Web.Mvc.Helpers;

public static class HtmlContentHelper
{
    public static IHtmlContent Process(
        string? rawHtml,
        Dictionary<string, string[]> requiredClasses
    )
    {
        if (string.IsNullOrWhiteSpace(rawHtml))
        {
            return HtmlString.Empty;
        }

        var parser = new HtmlParser();
        using var document = parser.ParseDocument($"<body>{rawHtml}</body>");

        foreach (var (tag, classes) in requiredClasses)
        {
            foreach (var element in document.QuerySelectorAll(tag))
            {
                foreach (var cls in classes)
                {
                    element.ClassList.Add(cls);
                }
            }
        }

        return new HtmlString(document.Body!.InnerHtml);
    }
}
