using SchoolAccount.Application.Abstractions.Messaging;

namespace SchoolAccount.Application.Services.GetServiceActionUrl;

public record GetServiceActionUrlQuery() : IQuery<GetServiceActionUrlResponse>;
