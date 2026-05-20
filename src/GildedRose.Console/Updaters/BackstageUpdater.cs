namespace GildedRose.Console.Updaters
{
    /// <summary>
    /// Updater for Backstage passes. Increases Quality as SellIn approaches,
    /// with special thresholds at 10 and 5 days; Quality drops to 0 after the concert.
    /// </summary>
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
}
