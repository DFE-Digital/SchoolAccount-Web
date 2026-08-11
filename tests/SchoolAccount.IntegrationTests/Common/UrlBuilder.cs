using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace SchoolAccount.IntegrationTests.Common;

[SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase")]
public static class UrlBuilder
{
    public static string GeneratePath(string? controller, string action, object? query = null)
    {
        return GeneratePath([controller?.Replace(nameof(Controller), string.Empty), action], query);
    }

    public static string GeneratePath(string action, object? query = null)
    {
        return GeneratePath([action], query);
    }

    public static string GeneratePath<T>(string action, object? query = null)
        where T : ControllerBase
    {
        var controllerType = typeof(T);
        return controllerType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Any(x => x.Name == action)
            ? GeneratePath(GetControllerRoute(controllerType), action, query)
            : throw new ArgumentException(
                $"Action {action} not found on controller {controllerType.Name}"
            );
    }

    private static string GetControllerRoute(Type controllerType)
    {
        var template = controllerType.GetCustomAttribute<RouteAttribute>()?.Template;

        if (template?.Contains('{') == true && template?.Contains('}') == true)
        {
            return null;
        }

        return template ?? controllerType.Name.Replace(nameof(Controller), string.Empty);
    }

    private static string GeneratePath(string?[] sections, object? query)
    {
        var path = ($"/" + string.Join("/", sections.Where(x => x is not null))).Replace("//", "/");
        var queryString = new List<string>();

        if (query is not null)
        {
            foreach (var property in query.GetType().GetProperties())
            {
                queryString.Add(
                    $"{property.Name}={WebUtility.UrlEncode(property.GetValue(query)?.ToString())}"
                );
            }
        }

        return query is null
            ? path.ToLowerInvariant()
            : $"{path.ToLowerInvariant()}?{string.Join("&", queryString)}";
    }
}
