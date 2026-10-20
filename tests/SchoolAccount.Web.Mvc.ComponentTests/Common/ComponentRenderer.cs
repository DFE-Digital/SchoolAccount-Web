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
/// Renders a single Razor view (a .cshtml file) on its own, using a model the test provides,
/// and returns the resulting HTML as a document you can search with CSS selectors.
/// </summary>
/// <remarks>
/// <para>
/// HOW IT FITS TOGETHER
/// </para>
/// <para>
/// Normally a browser sends a request, a controller builds a model, and MVC renders a view
/// with that model. Here we skip the browser, request and controller entirely and go
/// straight to the last step: "render this view with this model".
/// </para>
/// <para>
/// We inherit from <see cref="WebApplicationFactory{TEntryPoint}"/> (the same tool the
/// integration tests use) only to start the app in memory. That gives us the app's real,
/// fully configured services: the Razor view engine, tag helpers, routing and so on.
/// We never send it an HTTP request.
/// </para>
/// <para>
/// USAGE
/// </para>
/// <code>
/// public class MyComponentTests(ComponentRenderer renderer) : IClassFixture&lt;ComponentRenderer&gt;
/// {
///     [Fact]
///     public async Task Heading_says_hello()
///     {
///         using var html = await renderer.RenderAsync("~/Features/My/My.cshtml", model);
///         html.QuerySelector("h1").ShouldNotBeNull().TextContent.ShouldBe("Hello");
///     }
/// }
/// </code>
/// <para>
/// <c>IClassFixture</c> tells xUnit to create one renderer and share it between every
/// test in the class, so the app is only started once per test class.
/// </para>
/// </remarks>
public sealed class ComponentRenderer : WebApplicationFactory<Program>
{
    /// <summary>
    /// Renders the view at <paramref name="viewPath"/> with <paramref name="model"/> as its
    /// <c>@Model</c>, and returns the HTML it produced as a searchable document.
    /// </summary>
    /// <param name="viewPath">
    /// The view's path from the project root, starting with "~/",
    /// e.g. "~/Features/Shared/StepByStep/StepByStep.cshtml".
    /// </param>
    /// <param name="model">The data the view will see as <c>@Model</c>.</param>
    /// <param name="tempData">
    /// Optional. Values the view will find in <c>TempData</c>. TempData carries data across a
    /// redirect, e.g. validation errors after a form is posted. Leave out for none.
    /// </param>
    /// <param name="requestUrl">
    /// Optional. The URL of the page the view is pretending to be on, e.g. "/Journey?year=2026".
    /// Only needed by views that read the current URL. Defaults to "/".
    /// </param>
    public async Task<IDocument> RenderAsync<TModel>(
        string viewPath,
        TModel model,
        IDictionary<string, object>? tempData = null,
        string requestUrl = "/"
    )
    {
        // A "scope" is a short-lived container for services, the same as the app creates
        // for each real request. It's disposed (thrown away) at the end of this method.
        using var scope = Services.CreateScope();
        var services = scope.ServiceProvider;

        // Step 1: build the pretend request that Razor expects to exist.
        var actionContext = CreateActionContext(services, viewPath, requestUrl);

        // Step 2: ask the Razor view engine for the compiled view.
        var view = FindView(services, viewPath);

        // Step 3: run the view with our model, collecting its output as a string of HTML.
        var html = await RenderToStringAsync(services, actionContext, view, model, tempData);

        // Step 4: turn that string into a document we can search, like a browser would.
        return await ParseHtmlAsync(html);
    }

    /// <summary>
    /// Starts the app with the "IntegrationTests" settings (appsettings.IntegrationTests.json),
    /// which point at harmless local addresses instead of real external services.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
    }

    /// <summary>
    /// Builds the minimum "request context" Razor needs in order to render a view.
    /// </summary>
    /// <remarks>
    /// When a real controller returns <c>View(model)</c>, MVC passes the view a lot of context
    /// about the current request. Views use it for things like building links. We don't have
    /// a real request, so we build a blank one and fill in only the parts our views rely on.
    /// </remarks>
    private static ActionContext CreateActionContext(
        IServiceProvider services,
        string viewPath,
        string requestUrl
    )
    {
        // A blank HTTP request. "RequestServices" is where views and tag helpers look up any
        // services they need, so we point it at the app's real services.
        var httpContext = new DefaultHttpContext { RequestServices = services };

        // Set the URL the view will see in Context.Request. A URL needs a host to be parsed,
        // so we add a dummy one. Only the path ("/Journey") and query ("?year=2026") are used.
        var url = new Uri(new Uri("https://localhost"), requestUrl);
        httpContext.Request.Path = PathString.FromUriComponent(url);
        httpContext.Request.QueryString = QueryString.FromUriComponent(url);

        // Views build links with Url.Action(...) or asp-controller="..." / asp-action="...".
        // ASP.NET only uses the app's real routes to build those links if the request has an
        // "endpoint" (normally the controller action being run). Any endpoint will do, so we
        // give it a dummy one.
        httpContext.SetEndpoint(
            new Endpoint(
                requestDelegate: null,
                EndpointMetadataCollection.Empty,
                displayName: "Component test"
            )
        );

        // This project keeps views in feature folders (see FeatureConvention and
        // FeatureViewLocationExpander in the web project). At runtime each controller is tagged
        // with its folder name, e.g. "Start". That's how <partial name="_StartCards" /> inside
        // Start.cshtml knows to look in /Features/Start/. We have no controller, so we work out
        // the folder from the view's path and attach it ourselves.
        var actionDescriptor = new ControllerActionDescriptor
        {
            Properties = { [FeatureConvention.FeaturePropertyKey] = GetFeatureFolder(viewPath) },
        };

        return new ActionContext(httpContext, new RouteData(), actionDescriptor);
    }

    /// <summary>
    /// Finds the compiled view at <paramref name="viewPath"/>, or fails with a helpful message.
    /// </summary>
    private static IView FindView(IServiceProvider services, string viewPath)
    {
        var viewEngine = services.GetRequiredService<IRazorViewEngine>();

        // isMainPage: false means "render this as a partial". That stops Razor wrapping it in
        // _Layout.cshtml, so we get only this view's HTML, without the site header and footer.
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

    /// <summary>
    /// Runs the view with <paramref name="model"/> and returns the HTML it writes.
    /// </summary>
    private static async Task<string> RenderToStringAsync<TModel>(
        IServiceProvider services,
        ActionContext actionContext,
        IView view,
        TModel model,
        IDictionary<string, object>? tempDataValues
    )
    {
        // ViewData is where a view gets its data from. Setting ViewData.Model is what makes
        // our test data available as @Model inside the .cshtml file.
        var viewData = new ViewDataDictionary<TModel>(
            services.GetRequiredService<IModelMetadataProvider>(),
            new ModelStateDictionary()
        )
        {
            Model = model,
        };

        // In the real app TempData is stored in the user's session between requests. We have
        // no session, so we use a simple provider that hands the view the test's values.
        var tempData = new TempDataDictionary(
            actionContext.HttpContext,
            new InMemoryTempDataProvider(tempDataValues ?? new Dictionary<string, object>())
        );

        // Normally a view writes its HTML to the HTTP response. We give it a StringWriter
        // instead, so the HTML is collected in memory and we can read it back as a string.
        await using var writer = new StringWriter();

        var viewContext = new ViewContext(
            actionContext,
            view,
            viewData,
            tempData,
            writer,
            new HtmlHelperOptions()
        );

        // This runs the compiled .cshtml: its @if and @foreach blocks, HTML-encoding of values,
        // tag helpers such as <govuk-button-link>, and any <partial> it includes.
        await view.RenderAsync(viewContext);

        return writer.ToString();
    }

    /// <summary>
    /// Parses an HTML string into a document, the same way a browser would.
    /// </summary>
    /// <remarks>
    /// We could check the raw string with "Contains", but parsing is more reliable. It tells
    /// us what a browser would actually make of the HTML. For example, a typo like
    /// <c>&lt;tr lass="govuk-table__row"&gt;</c> still contains the text "govuk-table__row",
    /// but after parsing the row correctly has no classes at all.
    /// </remarks>
    private static async Task<IDocument> ParseHtmlAsync(string html)
    {
        var browsingContext = BrowsingContext.New(Configuration.Default);
        return await browsingContext.OpenAsync(request => request.Content(html));
    }

    /// <summary>
    /// Works out a view's feature folder from its path,
    /// e.g. "~/Features/Start/Start.cshtml" → "Start".
    /// </summary>
    private static string GetFeatureFolder(string viewPath)
    {
        // Everything before the last "/" is the folder: "~/Features/Start"
        var folder = viewPath[..viewPath.LastIndexOf('/')];

        // Remove the "~/Features/" prefix to leave just "Start"
        return folder.Replace("~/Features/", string.Empty, StringComparison.Ordinal);
    }

    /// <summary>
    /// Supplies TempData from a dictionary held in memory, instead of the user's session.
    /// </summary>
    /// <remarks>
    /// An <see cref="ITempDataProvider"/> has two jobs: load TempData when a request starts,
    /// and save it when the request ends. We only need the loading half. Saving does nothing,
    /// because there's no next request to save it for.
    /// </remarks>
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
