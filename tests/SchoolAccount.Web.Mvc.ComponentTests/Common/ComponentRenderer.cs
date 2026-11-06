using AngleSharp;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using SchoolAccount.Web.Mvc.Infrastructure;

namespace SchoolAccount.Web.Mvc.ComponentTests.Common;

/// <summary>
/// Renders a single Razor view with a test-supplied model and returns the parsed HTML.
/// </summary>
public sealed class ComponentRenderer : WebApplicationFactory<Program>
{
    private const string FeaturesRoot = "~/Features/";

    // xUnit runs a class's tests sequentially, so sharing this is safe.
    private readonly IBrowsingContext _browsingContext = BrowsingContext.New(Configuration.Default);

    public async Task<IDocument> RenderAsync<TModel>(
        string viewPath,
        TModel model,
        IDictionary<string, object>? tempData = null,
        string requestUrl = "/"
    )
    {
        using var scope = Services.CreateScope();
        var services = scope.ServiceProvider;

        var actionContext = CreateActionContext(services, viewPath, requestUrl);

        var view = FindView(services, viewPath);

        var html = await RenderToStringAsync(services, actionContext, view, model, tempData);

        // Parse rather than string-match so malformed markup can't give false positives.
        return await _browsingContext.OpenAsync(request => request.Content(html));
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
    }

    public override async ValueTask DisposeAsync()
    {
        _browsingContext.Dispose();
        await base.DisposeAsync();
    }

    private static ActionContext CreateActionContext(
        IServiceProvider services,
        string viewPath,
        string requestUrl
    )
    {
        var httpContext = new DefaultHttpContext { RequestServices = services };

        // Uri needs a host to parse; only the path and query are used.
        var url = new Uri(new Uri("https://localhost"), requestUrl);
        httpContext.Request.Path = PathString.FromUriComponent(url);
        httpContext.Request.QueryString = QueryString.FromUriComponent(url);

        // Link generation only uses the app's routes when the request has an endpoint.
        httpContext.SetEndpoint(
            new Endpoint(
                requestDelegate: null,
                EndpointMetadataCollection.Empty,
                displayName: "Component test"
            )
        );

        // Normally set per controller by FeatureConvention; partials need it to find their folder.
        var actionDescriptor = new ControllerActionDescriptor
        {
            Properties = { [FeatureConvention.FeaturePropertyKey] = GetFeatureFolder(viewPath) },
        };

        return new ActionContext(httpContext, new RouteData(), actionDescriptor);
    }

    private static IView FindView(IServiceProvider services, string viewPath)
    {
        var viewEngine = services.GetRequiredService<IRazorViewEngine>();

        // Render as a partial so _Layout isn't applied.
        var result = viewEngine.GetView(executingFilePath: null, viewPath, isMainPage: false);

        if (!result.Success)
        {
            throw new InvalidOperationException(
                $"Could not find view '{viewPath}'. Check the path starts with '~/' and ends with "
                    + $"'.cshtml'. Searched: {string.Join(", ", result.SearchedLocations)}"
            );
        }

        return result.View;
    }

    private static async Task<string> RenderToStringAsync<TModel>(
        IServiceProvider services,
        ActionContext actionContext,
        IView view,
        TModel model,
        IDictionary<string, object>? tempDataValues
    )
    {
        var viewData = new ViewDataDictionary<TModel>(
            services.GetRequiredService<IModelMetadataProvider>(),
            new ModelStateDictionary()
        )
        {
            Model = model,
        };

        var tempData = new TempDataDictionary(
            actionContext.HttpContext,
            new InMemoryTempDataProvider(tempDataValues ?? new Dictionary<string, object>())
        );

        await using var writer = new StringWriter();

        var viewContext = new ViewContext(
            actionContext,
            view,
            viewData,
            tempData,
            writer,
            new HtmlHelperOptions()
        );

        await view.RenderAsync(viewContext);

        return writer.ToString();
    }

    private static string GetFeatureFolder(string viewPath)
    {
        var lastSlash = viewPath.LastIndexOf('/');

        if (
            !viewPath.StartsWith(FeaturesRoot, StringComparison.Ordinal)
            || lastSlash < FeaturesRoot.Length
        )
        {
            throw new ArgumentException(
                $"View path '{viewPath}' must be a file inside a folder under '{FeaturesRoot}', "
                    + "e.g. '~/Features/Start/Start.cshtml'.",
                nameof(viewPath)
            );
        }

        return viewPath[FeaturesRoot.Length..lastSlash];
    }

    private sealed class InMemoryTempDataProvider(IDictionary<string, object> values)
        : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context)
        {
            return values;
        }

        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }
}
