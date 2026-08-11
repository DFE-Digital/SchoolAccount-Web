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

app.MapStaticAssets().AllowAnonymous();

app.MapControllerRoute("default", "{controller=Dashboard}/{action=Dashboard}/{id?}")
    .WithStaticAssets();

await app.RunAsync();
