using Microsoft.Extensions.Logging;

namespace SchoolAccount.Infrastructure.Http;

public sealed record HttpCallContext(ILogger Logger, Uri RequestUri);
