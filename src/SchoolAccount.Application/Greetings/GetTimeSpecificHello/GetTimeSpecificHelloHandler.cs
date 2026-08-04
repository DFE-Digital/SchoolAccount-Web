using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Greetings.GetTimeSpecificHello;

public class GetTimeSpecificHelloHandler(IDateTimeProvider dateTimeProvider)
    : IQueryHandler<GetTimeSpecificHelloQuery, GetTimeSpecificHelloResponse>
{
    public static class Messages
    {
        public const string Morning = "Good morning";
        public const string Afternoon = "Good afternoon";
        public const string Evening = "Good evening";
        public const string Night = "Good night (go to bed)";
    }

    public async Task<Result<GetTimeSpecificHelloResponse>> Handle(
        GetTimeSpecificHelloQuery query,
        CancellationToken cancellationToken
    )
    {
        var message = dateTimeProvider.UtcNow.Hour switch
        {
            >= 5 and < 12 => Messages.Morning,
            >= 12 and < 17 => Messages.Afternoon,
            >= 17 and < 22 => Messages.Evening,
            _ => Messages.Night,
        };

        var response = new GetTimeSpecificHelloResponse(message);

        return await Task.FromResult(response);
    }
}
