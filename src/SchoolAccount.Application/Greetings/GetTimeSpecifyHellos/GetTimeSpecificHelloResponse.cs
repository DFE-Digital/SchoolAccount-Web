namespace SchoolAccount.Application.Greetings.GetTimeSpecifyHellos;

public record GetTimeSpecificHelloResponse
{
    public GetTimeSpecificHelloResponse(int Hour, string Message)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(Hour, 24);
        ArgumentNullException.ThrowIfNull(Message);

        this.Hour = Hour;
        this.Message = Message;
    }

    public int Hour { get; init; }
    public string Message { get; init; }
}
