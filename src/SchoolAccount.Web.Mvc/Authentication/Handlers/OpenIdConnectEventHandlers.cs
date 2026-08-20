using Microsoft.AspNetCore.Authentication;
using SchoolAccount.Web.Mvc.Authentication.Extensions;

namespace SchoolAccount.Web.Mvc.Authentication.Handlers;

public static class OpenIdConnectEventHandlers
{
    public static Task OnTicketReceived(TicketReceivedContext context)
    {
        var org = context.Principal?.GetOrganisation();
        if (org is null)
        {
            context.Response.Redirect("/error/403");
            context.HandleResponse();
        }

        return Task.CompletedTask;
    }
}
