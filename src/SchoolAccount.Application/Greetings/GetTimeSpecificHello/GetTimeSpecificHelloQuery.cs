using SchoolAccount.Application.Abstractions.Messaging;

namespace SchoolAccount.Application.Greetings.GetTimeSpecificHello;

public record GetTimeSpecificHelloQuery() : IQuery<GetTimeSpecificHelloResponse>;
