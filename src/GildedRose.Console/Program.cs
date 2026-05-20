using GildedRose.Console.Updaters;

// NOTE: Per-item update logic has been refactored into the `Updaters` folder
// under the `GildedRose.Console.Updaters` namespace. Each updater implements
// `IItemUpdater` and encapsulates the rules for a specific item type. The
// `Program` class dispatches to these updaters from `UpdateQuality()`.
namespace GildedRose.Console;

public class Program
{
    public IList<Item> Items = [];
    // Strategy list of updaters used to apply per-item rules. DefaultUpdater must be last.
    //
    // Notes on refactor:
    // - Per-item rules (Aged Brie, Backstage, Sulfuras, Default) have been moved
    //   into `ItemUpdaters.cs` to make the logic easier to maintain and test.
    // - `Conjured` items are detected by name substring and handled by the
    //   DefaultUpdater (they degrade twice as fast).
    // - The `Updaters` list defines the dispatch order; the first matching
    //   updater wins. Keep `DefaultUpdater` as the final fallback.
    private static readonly List<IItemUpdater> Updaters =
    [
        new SulfurasUpdater(),
        new AgedBrieUpdater(),
        new BackstageUpdater(),
        new DefaultUpdater()
    ];

    static void Main(string[] args)
    {
        System.Console.WriteLine("OMGHAI!");

        var app = new Program()
                      {
                          Items =
                                      [
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
                                      ]

                      };

        app.UpdateQuality();

        System.Console.ReadKey();
    }

    public void UpdateQuality()
    {
        // Dispatch to the registered updaters (strategy pattern). The DefaultUpdater
        // is a catch-all and should always be present as the last updater.
        //
        // Behavior changes summary (kept intentionally small):
        // - Conjured items: detected by name, degrade in Quality twice as fast.
        // - Quality bounds and special-case rules have been centralized in
        //   ItemUpdaterHelpers.ChangeQuality to avoid duplication.
        foreach (var item in Items)
        {
            var updater = Updaters.Find(u => u.CanHandle(item));
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
