using GovUk.Frontend.AspNetCore;
using Microsoft.Extensions.Options;
using SchoolAccount.Application;
using SchoolAccount.Application.Collect.CensusStatus;
using SchoolAccount.Infrastructure;
using SchoolAccount.Web.Mvc;
using SchoolAccount.Web.Mvc.Config;
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

builder
    .Services.AddOptions<CommonApiConfig>()
    .Bind(builder.Configuration.GetSection("CommonApiSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHttpClient<ICollectApiService, CollectApiService>(
    (serviceProvider, client) =>
    {
        var config = serviceProvider.GetRequiredService<IOptions<CommonApiConfig>>().Value;
        client.BaseAddress = new Uri(config.CollectApiUrl);
    }
);

var app = builder.Build();

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
