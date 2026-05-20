using Xunit;
using GildedRose.Console;
using GildedRose.Console.Updaters;

namespace GildedRose.Tests;

// Focused tests for individual `IItemUpdater` implementations.
// These tests verify behavior after refactoring logic into updaters and
// specifically assert the Conjured item rule (double degradation) and
// centralized quality clamping.
public class ItemUpdaterFocusedTests
{
    // Aged Brie: increases in Quality by 1 each update and by an extra +1
    // once SellIn has passed. Also ensure Quality does not exceed the max.
    [Fact]
    public void AgedBrie_IncreasesQuality_And_RespectsMax()
    {
        var item = new Item { Name = "Aged Brie", SellIn = 5, Quality = 49 };
        new AgedBrieUpdater().Update(item);
        Assert.Equal(50, item.Quality);
        Assert.Equal(4, item.SellIn);
    }

    // Backstage passes: verify increases with thresholds (<=10, <=5) and
    // that Quality drops to 0 after the concert (SellIn < 0).
    [Fact]
    public void Backstage_IncreasesProperly_And_ResetsAfterConcert()
    {
        var item1 = new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 15, Quality = 20 };
        new BackstageUpdater().Update(item1);
        Assert.Equal(21, item1.Quality);

        var item2 = new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 10, Quality = 20 };
        new BackstageUpdater().Update(item2);
        Assert.Equal(22, item2.Quality);

        var item3 = new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 5, Quality = 20 };
        new BackstageUpdater().Update(item3);
        Assert.Equal(23, item3.Quality);

        var item4 = new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 0, Quality = 20 };
        new BackstageUpdater().Update(item4);
        Assert.Equal(0, item4.Quality);
    }

    // Sulfuras: legendary item never changes in Quality.
    [Fact]
    public void Sulfuras_DoesNotChange()
    {
        var item = new Item { Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80 };
        new SulfurasUpdater().Update(item);
        Assert.Equal(80, item.Quality);
    }

    // Default items: degrade normally and degrade twice as fast once sell date has passed.
    [Fact]
    public void Default_Degrades_And_AfterSellDateDegradesTwice()
    {
        var item = new Item { Name = "Elixir of the Mongoose", SellIn = 0, Quality = 10 };
        new DefaultUpdater().Update(item);
        Assert.Equal(8, item.Quality);
        Assert.Equal(-1, item.SellIn);
    }

    // Conjured items: detected by name and degrade twice as fast. Ensure not below zero.
    [Fact]
    public void Conjured_DegradesTwiceAndNotBelowZero()
    {
        var item = new Item { Name = "Conjured Mana Cake", SellIn = 3, Quality = 1 };
        new DefaultUpdater().Update(item);
        Assert.Equal(0, item.Quality);
        Assert.Equal(2, item.SellIn);
    }

    // Conjured items after sell date: should degrade 4 total in one update
    [Fact]
    public void Conjured_DegradesTwice_AfterSellDate()
    {
        var item = new Item { Name = "Conjured Mana Cake", SellIn = 0, Quality = 10 };
        new DefaultUpdater().Update(item);
        Assert.Equal(6, item.Quality);
        Assert.Equal(-1, item.SellIn);
    }
}
