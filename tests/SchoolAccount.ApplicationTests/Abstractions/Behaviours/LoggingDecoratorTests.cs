using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using NSubstitute;
using SchoolAccount.Application;
using SchoolAccount.Application.Abstractions.Clients;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Feedback;
using Shouldly;

namespace SchoolAccount.ApplicationTests.Abstractions.Behaviours;

public class LoggingDecoratorTests
{
    [Fact]
    public async Task Command_that_fails_validation_is_logged_as_an_error()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFakeLogging();
        services.AddApplication();
        services.AddSingleton(Substitute.For<IFeedbackClient>());
        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<FeedbackCommand>>();

        // Act
        await handler.Handle(
            new FeedbackCommand { Message = "test-message", OrganisationId = string.Empty },
            CancellationToken.None
        );

        // Assert
        provider
            .GetFakeLogCollector()
            .GetSnapshot()
            .ShouldContain(record =>
                record.Level == LogLevel.Error
                && record.Message == "Failed command FeedbackCommand with error"
            );
    }
}
