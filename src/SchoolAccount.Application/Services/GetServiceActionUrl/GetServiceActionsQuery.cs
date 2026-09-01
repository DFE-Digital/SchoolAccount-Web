using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel.Authentication;

namespace SchoolAccount.Application.Services.GetServiceActionUrl;

public record GetServiceActionsQuery : IQuery<GetServiceActionsResponse>
{
    public string Id { get; init; }

    public string EmailAddress { get; init; }

    public IReadOnlyList<Organisation> Organisations { get; init; } = [];
}
