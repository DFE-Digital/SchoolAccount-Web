using FluentValidation;

namespace SchoolAccount.Application.Features.Feedback;

public sealed class FeedbackCommandValidator : AbstractValidator<FeedbackCommand>
{
    public FeedbackCommandValidator()
    {
        RuleFor(x => x.Message).NotEmpty().MaximumLength(32000);

        RuleFor(x => x.OrganisationId).NotEmpty();
    }
}
