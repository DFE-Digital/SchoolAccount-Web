using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Greetings.GetTimeSpecifyHellos;

public class GetTimeSpecifyHellosHandler(IDateTimeProvider dateTimeProvider)
    : IQueryHandler<GetTimeSpecifyHellosQuery, GetTimeSpecifyHellosResponse>
{
    public static class Messages
    {
        public const string Morning = "Good morning";
        public const string Afternoon = "Good afternoon";
        public const string Evening = "Good evening";
        public const string Night = "Good night (go to bed)";
    }

    public async Task<Result<GetTimeSpecifyHellosResponse>> Handle(
        GetTimeSpecifyHellosQuery query,
        CancellationToken cancellationToken
    )
    {
        string message = dateTimeProvider.UtcNow.Hour switch
        {
            >= 5 and < 12 => Messages.Morning,
            >= 12 and < 17 => Messages.Afternoon,
            >= 17 and < 22 => Messages.Evening,
            _ => Messages.Night,
        };

        return new GetTimeSpecifyHellosResponse(dateTimeProvider.UtcNow.Hour, message);
    }
}
