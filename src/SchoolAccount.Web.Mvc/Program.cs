using GovUk.Frontend.AspNetCore;
using SchoolAccount.Application;
using SchoolAccount.Infrastructure;
using SchoolAccount.Web.Mvc;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication().AddPresentation().AddInfrastructure();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddGovUkFrontend();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSassCompiler();
}

WebApplication app = builder.Build();

app.UseStatusCodePagesWithReExecute("/Error/{0}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/500");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseGovUkFrontend();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await app.RunAsync();
