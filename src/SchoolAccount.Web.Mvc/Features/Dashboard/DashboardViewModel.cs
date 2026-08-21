namespace SchoolAccount.Web.Mvc.Features.Dashboard;

public record DashboardViewModel(
    string User,
    string GreetingsMessage,
    List<string> censusGreetings
);
