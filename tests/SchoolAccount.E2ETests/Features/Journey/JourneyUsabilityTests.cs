using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Playwright;
using SchoolAccount.E2ETests.Common.Extensions;
using SchoolAccount.E2ETests.Common.Fixtures;
using Shouldly;

namespace SchoolAccount.E2ETests.Features.Journey;

[Collection(SchoolAccountCollection.Name)]
public class JourneyUsabilityTests : BrowserTestBase
{
    private const string FocusedElementDescriptorScript = """
        el => {
            const role = el.getAttribute('role') || el.tagName.toLowerCase();
            let name = (el.getAttribute('aria-label') || el.textContent || el.value || '').trim();
            if (!name) {
                const img = el.querySelector('img[alt]');
                if (img) {
                    name = img.getAttribute('alt').trim();
                }
            }
            return role + ':' + name.replace(/\s+/g, ' ');
        }
        """;

    private const string SnapshotFileName = "Journey.tab-order.json";
    private const string ReceivedFileName = "Journey.tab-order.received.json";
    private const int MaximumTabPresses = 60;

    private static readonly JsonSerializerOptions SnapshotJsonOptions = new()
    {
        WriteIndented = true,
    };

    public JourneyUsabilityTests(SchoolAccountBrowserFixture fixture)
        : base(fixture) { }

    protected override async Task OnInitializedAsync()
    {
        await Page.GotoAsync($"{Settings.SiteUrl}/Journey");
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

        for (var i = 0; i < MaximumTabPresses; i++)
        {
            await Page.Keyboard.PressAsync("Tab");

            string current;
            try
            {
                current = await Page.Locator(":focus")
                    .EvaluateAsync<string>(
                        FocusedElementDescriptorScript,
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
        var snapshotPath = SnapshotPath(SnapshotFileName);
        var receivedPath = SnapshotPath(ReceivedFileName);

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
            JsonSerializer.Serialize(tabOrder, SnapshotJsonOptions),
            cancellationToken
        );

        savedTabOrder.ShouldNotBeNull(
            $"There is no saved tab order ({SnapshotFileName}). The order found has been written to "
                + $"{ReceivedFileName}: review it, then rename it to {SnapshotFileName} to accept it."
        );
        tabOrder.ShouldBe(
            savedTabOrder,
            $"The tab order has changed. The new order has been written to {ReceivedFileName}: "
                + $"if the change is intended, review it and rename it to {SnapshotFileName}."
        );
    }

    private static string SnapshotPath(string fileName, [CallerFilePath] string sourceFile = "") =>
        Path.Combine(Path.GetDirectoryName(sourceFile)!, fileName);
}
