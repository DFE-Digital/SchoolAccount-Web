using GovUk.Frontend.AspNetCore;
using SchoolAccount.Application;
using SchoolAccount.Infrastructure;
using SchoolAccount.Web.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddApplication()
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
app.UseGovUkFrontend();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await app.RunAsync();
