using SchoolAccount.Application.Abstractions.Messaging;

namespace SchoolAccount.Application.Services.GetServiceActionUrl;

public record GetServiceActionsQuery() : IQuery<GetServiceActionsResponse>;
