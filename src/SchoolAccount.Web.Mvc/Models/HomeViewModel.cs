namespace SchoolAccount.Web.Mvc.Models;

public class HomeViewModel(string greetingMessage)
{
    public string GreetingMessage { get; init; } = greetingMessage;
}
