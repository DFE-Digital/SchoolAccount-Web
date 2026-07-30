using SchoolAccount.Application.Abstractions.Messaging;

namespace SchoolAccount.Application.Greetings.GetTimeSpecifyHellos;

public record GetTimeSpecificHelloQuery() : IQuery<GetTimeSpecificHelloResponse>;
