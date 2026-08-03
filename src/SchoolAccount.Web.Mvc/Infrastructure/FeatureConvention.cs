using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace SchoolAccount.Web.Mvc.Infrastructure;

/// <summary>
/// Tags each controller with the feature folder it belongs to, derived from the
/// namespace segments after "Features" (e.g. Features.Tasks.GetAll → "Tasks/GetAll").
/// Used by <see cref="FeatureViewLocationExpander"/> to resolve views by feature folder.
/// </summary>
public sealed class FeatureConvention : IControllerModelConvention
{
    public const string FeaturePropertyKey = "feature";

    public void Apply(ControllerModel controller)
    {
        controller.Properties[FeaturePropertyKey] = GetFeaturePath(controller.ControllerType);
    }

    private static string GetFeaturePath(TypeInfo controllerType)
    {
        var tokens = (controllerType.Namespace ?? string.Empty).Split('.');

        return string.Join('/', tokens.SkipWhile(token => token != "Features").Skip(1));
    }
}
