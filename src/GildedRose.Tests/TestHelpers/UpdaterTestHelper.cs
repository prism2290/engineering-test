using GildedRose.Console.Updaters;

namespace GildedRose.Tests.TestHelpers;

/// <summary>
/// Test helper that provides the default, priority-ordered set of updaters
/// used by the application. Tests can reuse this to ensure consistent
/// behavior with the runtime registration order.
/// </summary>
public static class UpdaterTestHelper
{
    // Returns the default ordered set of updaters used in the application.
    public static IEnumerable<IItemUpdater> CreateDefaultUpdaters()
    {
        return new List<IItemUpdater>
        {
            new SulfurasUpdater(),
            new AgedBrieUpdater(),
            new BackstageUpdater(),
            new DefaultUpdater()
        };
    }
}
