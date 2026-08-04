namespace SchoolAccount.Web.Mvc.Features.Home;

public class HomeViewModel(string greetingMessage)
{
    public string GreetingMessage { get; init; } = greetingMessage;
}
