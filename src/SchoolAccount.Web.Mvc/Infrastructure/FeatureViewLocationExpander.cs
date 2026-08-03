using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;

namespace SchoolAccount.Web.Mvc.Infrastructure;

/// <summary>
/// Expands the "{3}" token in view location formats to the controller's feature folder
/// path (set by <see cref="FeatureConvention"/>), so actions can return View("Name")
/// and have it resolve to /Features/&lt;feature&gt;/Name.cshtml.
/// </summary>
public sealed class FeatureViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
        // Part of the view lookup cache key, so each feature caches its own locations
        context.Values[FeatureConvention.FeaturePropertyKey] = GetFeaturePath(context);
    }

    public IEnumerable<string> ExpandViewLocations(
        ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations
    )
    {
        var featurePath = GetFeaturePath(context);

        foreach (var location in viewLocations)
        {
            if (!location.Contains("{3}", StringComparison.Ordinal))
            {
                yield return location;
            }
            else if (!string.IsNullOrEmpty(featurePath))
            {
                yield return location.Replace("{3}", featurePath, StringComparison.Ordinal);
            }
        }
    }

    private static string? GetFeaturePath(ViewLocationExpanderContext context)
    {
        return
            context.ActionContext.ActionDescriptor is ControllerActionDescriptor descriptor
            && descriptor.Properties.TryGetValue(
                FeatureConvention.FeaturePropertyKey,
                out var feature
            )
            ? feature as string
            : null;
    }
}
