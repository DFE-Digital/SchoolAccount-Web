using GovUk.Frontend.AspNetCore;
using SchoolAccount.Application;
using SchoolAccount.Infrastructure;
using SchoolAccount.Web.Mvc;
using SchoolAccount.Web.Mvc.Hosting.Extensions;
using SchoolAccount.Web.Mvc.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfiguredAzureAppConfiguration();

builder.Logging.ClearProviders();
builder.Host.UseConfiguredSerilog();

builder
    .Services.AddApplication()
    .AddPresentation(builder.Environment, builder.Configuration)
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseForwardedHeadersDiagnostics(app.Environment);
app.UseConfiguredForwardedHeaders(app.Configuration);
app.UseStatusCodePagesWithReExecute("/error/{0}");
app.UseExceptionHandler("/error/500");
app.UseSerilogRequestLogging();
app.UseConfiguredAzureAppConfiguration();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseMiddleware<TrackingCookieMiddleware>();
app.UseSession();
app.UseGovUkFrontend();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets().AllowAnonymous();

app.MapControllerRoute("default", "/{controller}/{action}").WithStaticAssets();

await app.RunAsync();
