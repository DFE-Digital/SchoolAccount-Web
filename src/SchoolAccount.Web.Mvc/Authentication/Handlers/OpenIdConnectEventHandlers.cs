using Microsoft.AspNetCore.Authentication;

namespace SchoolAccount.Web.Mvc.Authentication.Handlers;

public static class OpenIdConnectEventHandlers
{
    public static Task OnTicketReceived(TicketReceivedContext context)
    {
        var org = context.Principal?.FindFirst(ClaimConstants.Organisation)?.Value;
        if (string.IsNullOrEmpty(org))
        {
            context.Response.Redirect("/error/403");
            context.HandleResponse();
        }

        return Task.CompletedTask;
    }
}
