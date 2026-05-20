namespace GildedRose.Console.Updaters
{
    /// <summary>
    /// Updater for "Aged Brie" which increases in Quality as it ages.
    /// Uses <see cref="ItemUpdaterHelpers.ChangeQuality"/> to respect bounds.
    /// </summary>
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
}
