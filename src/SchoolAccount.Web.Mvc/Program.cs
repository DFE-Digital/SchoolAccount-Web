using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using SchoolAccount.Application;
using SchoolAccount.Infrastructure;
using SchoolAccount.Web.Mvc;
using SchoolAccount.Web.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureAppConfiguration();
}

builder
    .Services.AddApplication()
    .AddPresentation(builder.Environment, builder.Configuration)
    .AddInfrastructure();

var app = builder.Build();

// Trusts the X-Forwarded-Proto/X-Forwarded-For headers set by the reverse proxy
// (Caddy locally, Azure Container Apps ingress in production) so ASP.NET Core
// knows the original request was HTTPS, even though it reaches Kestrel over HTTP.
// KnownNetworks/KnownProxies default to loopback-only, which the proxy's Docker
// network IP won't match, so they're cleared to trust any proxy hop instead -
// the proxy's IP isn't known ahead of time in either environment.
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
};
forwardedHeadersOptions.KnownIPNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardedHeadersOptions);

app.UseStatusCodePagesWithReExecute("/error/{0}");
app.UseExceptionHandler("/error/500");

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

app.MapControllerRoute("default", "/{controller}/{action}").WithStaticAssets();

await app.RunAsync();
