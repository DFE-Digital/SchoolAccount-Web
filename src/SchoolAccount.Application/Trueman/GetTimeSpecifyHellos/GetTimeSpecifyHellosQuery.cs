using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.SharedKernel;

namespace SchoolAccount.Application.Trueman.GetTimeSpecifyHellos;

public record GetTimeSpecifyHellosQuery() : IQuery<GetTimeSpecifyHellosResponse>;
