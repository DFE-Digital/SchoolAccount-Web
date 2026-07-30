using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Trueman.GetTimeSpecifyHellos;

public class GetTimeSpecifyHellosHandler(IDateTimeProvider dateTimeProvider)
    : IQueryHandler<GetTimeSpecifyHellosQuery, GetTimeSpecifyHellosResponse>
{
    public async Task<Result<GetTimeSpecifyHellosResponse>> Handle(
        GetTimeSpecifyHellosQuery query,
        CancellationToken cancellationToken
    )
    {
        string message = dateTimeProvider.UtcNow.Hour switch
        {
            >= 5 and < 12 => "Good morning",
            >= 12 and < 17 => "Good afternoon",
            >= 17 and < 22 => "Good evening",
            _ => "Good night (go to bed)",
        };

        return new GetTimeSpecifyHellosResponse(dateTimeProvider.UtcNow.Hour, message);
    }
}
