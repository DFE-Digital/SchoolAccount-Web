using System.Net;
using System.Net.Http.Headers;
using NSubstitute;
using SchoolAccount.Application.Abstractions.Messaging;
using SchoolAccount.Application.Features.Feedback;
using SchoolAccount.IntegrationTests.Common;
using SchoolAccount.IntegrationTests.Common.Pages;
using SchoolAccount.SharedKernel;
using Shouldly;

namespace SchoolAccount.IntegrationTests.Features.Feedback;

public class FeedbackControllerTests : IClassFixture<SchoolAccountWebApplicationFactory<Program>>
{
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
    private readonly SchoolAccountWebApplicationFactory<Program> _factory;
    private readonly HttpClient _authenticatedClient;
    private readonly ICommandHandler<FeedbackCommand> _feedbackCommandHandler = Substitute.For<
        ICommandHandler<FeedbackCommand>
    >();

    public FeedbackControllerTests(SchoolAccountWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _authenticatedClient = factory.CreateAuthorisedClient(services =>
            services.AddScoped<ICommandHandler<FeedbackCommand>>(_ => _feedbackCommandHandler)
        );
    }

    [Fact]
    public async Task Submit_successfully_performs_local_redirect()
    {
        // Arrange
        var pageUri = _factory.GeneratePath("Feedback", "Submit");
        _feedbackCommandHandler
            .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success);

        // Act
        using var content = new StringContent(string.Empty);
        var result = await _authenticatedClient.PostAsync(pageUri, content, _cancellationToken);

        // Assert
        result.Headers.Location?.OriginalString.ShouldEndWith("#feedback-submitted");
    }

    // [Fact]
    // public async Task Submit_returns_correct_partial_view()
    // {
    //     // Arrange
    //     var pageUri = _factory.GeneratePath("Feedback", "Submit");
    //     _feedbackCommandHandler
    //         .Handle(Arg.Any<FeedbackCommand>(), Arg.Any<CancellationToken>())
    //         .Returns(Result.Success);
    //
    //     // Act
    //     using var content = new StringContent("XMLHttpRequest");
    //     var result = await _authenticatedClient.PostAsync(pageUri, content, _cancellationToken);
    //
    //     // Assert
    // }
}
