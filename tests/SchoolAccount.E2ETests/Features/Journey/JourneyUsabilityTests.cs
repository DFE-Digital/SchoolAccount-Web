using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Playwright;
using SchoolAccount.E2ETests.Common.Extensions;
using SchoolAccount.E2ETests.Common.Fixtures;
using SchoolAccount.TestCommon.Constants;
using Shouldly;

namespace SchoolAccount.E2ETests.Features.Journey;

[Collection(SchoolAccountCollection.Name)]
public class JourneyUsabilityTests : BrowserTestBase
{
    private const string _focusedElementDescriptorScript = """
        el => {
            const role = el.getAttribute('role') || el.tagName.toLowerCase();
            let name = (el.getAttribute('aria-label') || el.textContent || el.value || '').trim();
            if (!name) {
                const img = el.querySelector('img[alt]');
                if (img) {
                    name = img.getAttribute('alt').trim();
                }
            }
            // The census season changes during the year, so it's swapped for a placeholder.
            name = name.replace(/\s+/g, ' ').replace(/\b(Autumn|Spring|Summer)\b/g, '[season]');
            return role + ':' + name;
        }
        """;

    private const string _snapshotFileName = "Journey.tab-order.json";
    private const string _receivedFileName = "Journey.tab-order.received.json";
    private const int _maximumTabPresses = 60;

    private static readonly JsonSerializerOptions _snapshotJsonOptions = new()
    {
        WriteIndented = true,
    };

    public JourneyUsabilityTests(SchoolAccountBrowserFixture fixture)
        : base(fixture) { }

    protected override async Task OnInitializedAsync()
    {
        await Page.GotoAsync($"{Settings.SiteUrl}{UrlConstants.JourneyUrl}");
    }

    [Fact]
    public async Task Every_visible_interactive_element_has_an_accessible_name()
    {
        // Act
        var elementsWithoutNames = await Page.FindElementsWithoutAccessibleNamesAsync();

        // Assert
        elementsWithoutNames.ShouldBeEmpty();
    }

    [Fact]
    public async Task Tab_order_matches_the_saved_snapshot()
    {
        // Act
        var tabOrder = await CaptureTabOrderAsync();

        // Assert
        await ShouldMatchSavedTabOrderAsync(tabOrder);
    }

    /// <summary>
    /// Presses Tab until focus leaves the page or comes back round to the first element,
    /// recording each element that receives focus.
    /// </summary>
    private async Task<List<string>> CaptureTabOrderAsync()
    {
        var tabOrder = new List<string>();
        string? first = null;

        for (var i = 0; i < _maximumTabPresses; i++)
        {
            await Page.Keyboard.PressAsync("Tab");

            string current;
            try
            {
                current = await Page.Locator(":focus")
                    .EvaluateAsync<string>(
                        _focusedElementDescriptorScript,
                        arg: null,
                        new LocatorEvaluateOptions { Timeout = 1000 }
                    );
            }
            catch (TimeoutException)
            {
                break;
            }

            if (current == first)
            {
                break;
            }

            first ??= current;
            tabOrder.Add(current);
        }

        return tabOrder;
    }

    /// <summary>
    /// Fails when there's no saved tab order or it differs from <paramref name="tabOrder"/>,
    /// writing the order found to a ".received" file beside the snapshot for review.
    /// </summary>
    private static async Task ShouldMatchSavedTabOrderAsync(List<string> tabOrder)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var snapshotPath = SnapshotPath(_snapshotFileName);
        var receivedPath = SnapshotPath(_receivedFileName);

        var savedTabOrder = File.Exists(snapshotPath)
            ? JsonSerializer.Deserialize<List<string>>(
                await File.ReadAllTextAsync(snapshotPath, cancellationToken)
            )
            : null;

        if (savedTabOrder is not null && savedTabOrder.SequenceEqual(tabOrder))
        {
            File.Delete(receivedPath);
            return;
        }

        await File.WriteAllTextAsync(
            receivedPath,
            JsonSerializer.Serialize(tabOrder, _snapshotJsonOptions),
            cancellationToken
        );

        savedTabOrder.ShouldNotBeNull(
            $"There is no saved tab order ({_snapshotFileName}). The order found has been written to "
                + $"{_receivedFileName}: review it, then rename it to {_snapshotFileName} to accept it."
        );
        tabOrder.ShouldBe(
            savedTabOrder,
            $"The tab order has changed. The new order has been written to {_receivedFileName}: "
                + $"if the change is intended, review it and rename it to {_snapshotFileName}."
        );
    }

    private static string SnapshotPath(string fileName, [CallerFilePath] string sourceFile = "")
    {
        return Path.Combine(Path.GetDirectoryName(sourceFile)!, fileName);
    }
}
