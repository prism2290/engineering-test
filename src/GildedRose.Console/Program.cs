using GildedRose.Console.Updaters;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Collections.Generic;

// NOTE: Per-item update logic has been refactored into the `Updaters` folder
// under the `GildedRose.Console.Updaters` namespace. Each updater implements
// `IItemUpdater` and encapsulates the rules for a specific item type. The
// `Program` class dispatches to these updaters from `UpdateQuality()`.
namespace GildedRose.Console;

/// <summary>
/// Application entry point and dispatcher for item update logic.
/// Constructed with a set of <see cref="Updaters.IItemUpdater"/> instances
/// so callers can provide deterministic behaviors (useful for tests and DI).
/// </summary>
public class Program
{
    public IList<Item> Items = new List<Item>();

    // Updaters are injected via DI (order registered defines dispatch priority).
    private readonly List<IItemUpdater> _updaters;

    public Program(IEnumerable<IItemUpdater> updaters)
    {
        // Require an explicit set of updaters to make tests and DI usage
        // deterministic. Order of registration determines dispatch priority
        // (first matching updater wins). Throw early on misconfiguration.
        if (updaters == null) throw new System.ArgumentNullException(nameof(updaters));
        _updaters = updaters.ToList();
    }

    static void Main(string[] args)
    {
        System.Console.WriteLine("OMGHAI!");

        var services = new ServiceCollection();
        // Register updaters in priority order. The first registered that matches
        // an item will be used; keep DefaultUpdater last as the fallback.
        services.AddSingleton<IItemUpdater, SulfurasUpdater>();
        services.AddSingleton<IItemUpdater, AgedBrieUpdater>();
        services.AddSingleton<IItemUpdater, BackstageUpdater>();
        services.AddSingleton<IItemUpdater, DefaultUpdater>();

        // Register Program so its constructor receives the updaters.
        services.AddSingleton<Program>();

        var provider = services.BuildServiceProvider();
        var app = provider.GetRequiredService<Program>();

        app.Items = new List<Item>
        {
            new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
            new Item {Name = "Aged Brie", SellIn = 2, Quality = 0},
            new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
            new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
            new Item
            {
                Name = "Backstage passes to a TAFKAL80ETC concert",
                SellIn = 15,
                Quality = 20
            },
            new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
        };

        app.UpdateQuality();

        System.Console.ReadKey();
    }

    /// <summary>
    /// Run a single update cycle for every item by dispatching to the first
    /// matching registered updater.
    /// </summary>
    /// <remarks>
    /// The order of <c>_updaters</c> defines priority; keep <c>DefaultUpdater</c>
    /// last so it acts as a fallback. Use <see cref="Updaters.ItemUpdaterHelpers.ChangeQuality"/>
    /// inside updaters to preserve invariants (0..50 quality bounds and Sulfuras rules).
    /// </remarks>
    public void UpdateQuality()
    {
        foreach (var item in Items)
        {
            var updater = _updaters.Find(u => u.CanHandle(item));
            if (updater == null) continue; // defensive: DefaultUpdater should handle all
            updater.Update(item);
        }
    }
    
}

public class Item
{
    public string Name { get; set; } = "";

    public int SellIn { get; set; }

    public int Quality { get; set; }
}
