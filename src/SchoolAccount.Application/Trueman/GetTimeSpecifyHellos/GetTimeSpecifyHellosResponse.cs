namespace SchoolAccount.Application.Trueman.GetTimeSpecifyHellos;

public record GetTimeSpecifyHellosResponse
{
    public GetTimeSpecifyHellosResponse(int Hour, string Message)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(Hour, 24);
        ArgumentNullException.ThrowIfNull(Message);

        this.Hour = Hour;
        this.Message = Message;
    }

    public int Hour { get; init; }
    public string Message { get; init; }
}
