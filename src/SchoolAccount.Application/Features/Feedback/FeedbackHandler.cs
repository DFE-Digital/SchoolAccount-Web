using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Features.Feedback;

public class FeedbackHandler(IAzureTableClient azureTableClient) : ICommandHandler<FeedbackCommand>
{
    public async Task<Result> Handle(FeedbackCommand command, CancellationToken cancellationToken)
    {
        var result = await azureTableClient.SendFeedback(
            command.Message,
            command.Ukprn,
            command.OrganisationId
        );

        return result;
    }
}
