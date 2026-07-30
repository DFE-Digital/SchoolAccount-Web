namespace SchoolAccount.Application.Greetings.GetTimeSpecificHello;

public record GetTimeSpecificHelloResponse
{
    public GetTimeSpecificHelloResponse(string Message)
    {
        ArgumentNullException.ThrowIfNull(Message);

        this.Message = Message;
    }

    public string Message { get; init; }
}
