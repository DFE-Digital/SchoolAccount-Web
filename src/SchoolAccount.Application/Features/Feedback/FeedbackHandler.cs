using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Features.Feedback;

public class FeedbackHandler(IFeedbackClient feedbackClient) : ICommandHandler<FeedbackCommand>
{
    public async Task<Result> Handle(FeedbackCommand command, CancellationToken cancellationToken)
    {
        var result = await feedbackClient.SendFeedback(
            command.Message,
            command.Ukprn,
            command.OrganisationId,
            cancellationToken
        );

        return result;
    }
}
