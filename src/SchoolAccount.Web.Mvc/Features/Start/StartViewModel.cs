namespace SchoolAccount.Web.Mvc.Features.Start;

public class StartViewModel(string greetingMessage)
{
    public string GreetingMessage { get; init; } = greetingMessage;
}
