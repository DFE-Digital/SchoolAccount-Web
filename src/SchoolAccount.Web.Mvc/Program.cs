using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc.Controllers;
using SchoolAccount.Application;
using SchoolAccount.Infrastructure;
using SchoolAccount.Web.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddApplication()
    .AddSession()
    .AddPresentation(builder.Environment, builder.Configuration)
    .AddInfrastructure();

var app = builder.Build();

app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseExceptionHandler("/Error/500");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseGovUkFrontend();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute("default", "{controller=Dashboard}/{action=Dashboard}/{id?}")
    .WithStaticAssets();

app.MapGet(
    "/debug/routes",
    (IEnumerable<EndpointDataSource> endpointSources) =>
    {
        var routes = endpointSources
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Select(e =>
            {
                var httpMethods = e
                    .Metadata.OfType<HttpMethodMetadata>()
                    .FirstOrDefault()
                    ?.HttpMethods;

                var controllerAction = e
                    .Metadata.OfType<ControllerActionDescriptor>()
                    .FirstOrDefault();

                return new
                {
                    Methods = string.Join(",", httpMethods ?? [string.Empty]),
                    Route = e.RoutePattern.RawText!,
                    Action = controllerAction != null
                        ? $"{controllerAction.ControllerName}.{controllerAction.ActionName}"
                        : string.Empty,
                };
            })
            .ToList();

        // Calculate max widths for formatting
        var methodWidth = routes.Max(r => r.Methods.Length) + 2;
        var routeWidth = routes.Max(r => r.Route.Length) + 2;
        var actionWidth = routes.Max(r => r.Action.Length) + 2;

        // Format output like a table
        var header =
            $"{"Route".PadRight(routeWidth)}{"Method".PadRight(methodWidth)}{"Action".PadRight(actionWidth)}";
        var divider = new string('-', header.Length);
        var body = string.Join(
            "\n",
            routes
                .OrderBy(x => x.Route)
                .Select(r =>
                    $"{r.Route.PadRight(routeWidth)}{r.Methods.PadRight(methodWidth)}{r.Action.PadRight(actionWidth)}"
                )
        );

        return $"{header}\n{divider}\n{body}";
    }
);

await app.RunAsync();
