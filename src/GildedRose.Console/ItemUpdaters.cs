using System;

namespace GildedRose.Console;

// Item updaters implement per-item business rules and are invoked by the
// dispatcher in Program.UpdateQuality. This file centralizes those rules so
// they can be reasoned about and tested independently.

public interface IItemUpdater
{
    // Returns true if this updater should be used for the supplied item.
    bool CanHandle(Item item);

    // Applies the rules for the item (adjusts Quality and SellIn as needed).
    void Update(Item item);
}

internal static class ItemUpdaterHelpers
{
    // Centralized quality bounds used by all updaters.
    public const int MinQuality = 0;
    public const int MaxQuality = 50;

    // Predicates to identify special items. Keep these simple and explicit
    // so rules remain easy to follow.
    public static bool IsAgedBrie(Item it) => it.Name == "Aged Brie";
    public static bool IsBackstage(Item it) => it.Name == "Backstage passes to a TAFKAL80ETC concert";
    public static bool IsSulfuras(Item it) => it.Name == "Sulfuras, Hand of Ragnaros";

    // Conjured items degrade twice as fast; detection is name-based for this kata.
    public static bool IsConjured(Item it) => !string.IsNullOrEmpty(it.Name) && it.Name.Contains("Conjured", StringComparison.OrdinalIgnoreCase);

    // ChangeQuality enforces the min/max bounds and skips Sulfuras (legendary).
    // Centralizing this prevents duplicated bound-checking logic across updaters.
    public static void ChangeQuality(Item it, int delta)
    {
        if (IsSulfuras(it)) return;
        var q = it.Quality + delta;
        if (q < MinQuality) q = MinQuality;
        if (q > MaxQuality) q = MaxQuality;
        it.Quality = q;
    }
}

// Sulfuras - legendary item: never changes.
public class SulfurasUpdater : IItemUpdater
{
    public bool CanHandle(Item item) => ItemUpdaterHelpers.IsSulfuras(item);
    public void Update(Item item) { /* legendary item - no changes */ }
}

// Aged Brie increases in Quality the older it gets.
public class AgedBrieUpdater : IItemUpdater
{
    public bool CanHandle(Item item) => ItemUpdaterHelpers.IsAgedBrie(item);
    public void Update(Item item)
    {
        ItemUpdaterHelpers.ChangeQuality(item, +1);
        item.SellIn -= 1;
        if (item.SellIn < 0) ItemUpdaterHelpers.ChangeQuality(item, +1);
    }
}

// Backstage passes increase in Quality as the SellIn approaches; after the
// concert (SellIn < 0) quality drops to zero.
public class BackstageUpdater : IItemUpdater
{
    public bool CanHandle(Item item) => ItemUpdaterHelpers.IsBackstage(item);
    public void Update(Item item)
    {
        ItemUpdaterHelpers.ChangeQuality(item, +1);
        if (item.SellIn < 11) ItemUpdaterHelpers.ChangeQuality(item, +1);
        if (item.SellIn < 6) ItemUpdaterHelpers.ChangeQuality(item, +1);
        item.SellIn -= 1;
        if (item.SellIn < 0) item.Quality = 0;
    }
}

// Default updater handles normal items. It also implements Conjured behavior
// by doubling the decrement when `IsConjured` is true.
public class DefaultUpdater : IItemUpdater
{
    public bool CanHandle(Item item) => true; // catch-all
    public void Update(Item item)
    {
        var decrement = ItemUpdaterHelpers.IsConjured(item) ? 2 : 1;
        ItemUpdaterHelpers.ChangeQuality(item, -decrement);
        item.SellIn -= 1;
        if (item.SellIn < 0) ItemUpdaterHelpers.ChangeQuality(item, -decrement);
    }
}
