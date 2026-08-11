using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Greetings.GetTimeSpecificHello;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Web.Mvc.Features.Start;

[Route("/")]
[SuppressMessage("Design", "CA1054:URI-like parameters should not be strings")]
public class StartController : Controller
{
    [HttpGet(""), AllowAnonymous]
    public async Task<IActionResult> Start(string? returnUrl)
    {
        return View(new StartViewModel(returnUrl));
    }
}
