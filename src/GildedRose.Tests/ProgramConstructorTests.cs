using Xunit;
using GildedRose.Console;
using GildedRose.Tests.TestHelpers;

namespace GildedRose.Tests;

public class ProgramConstructorTests
{
    [Fact]
    public void Program_ConstructedWithUpdaters_UpdatesItemsCorrectly()
    {
        // Arrange: create updaters in priority order and inject into Program
        var program = new Program(UpdaterTestHelper.CreateDefaultUpdaters());

        program.Items = new List<Item>
        {
            new Item { Name = "Aged Brie", SellIn = 2, Quality = 0 },
            new Item { Name = "Conjured Mana Cake", SellIn = 3, Quality = 6 }
        };

        // Act
        program.UpdateQuality();

        // Assert: Aged Brie increases, Conjured degrades twice as fast.
        Assert.Equal(1, program.Items[0].Quality);
        Assert.Equal(4, program.Items[1].Quality);
    }
}
