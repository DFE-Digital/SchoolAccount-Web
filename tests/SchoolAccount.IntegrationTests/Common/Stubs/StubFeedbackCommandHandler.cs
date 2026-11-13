using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.IntegrationTests.Common.Stubs;

public sealed class StubFeedbackCommandHandler : StubCommandHandler<FeedbackCommand>
{
    public static StubFeedbackCommandHandler Succeeding()
    {
        var handler = new StubFeedbackCommandHandler();
        handler.Returns(Result.Success());

        return handler;
    }
}
