namespace GildedRose.Console.Updaters
{
    /// <summary>
    /// Default updater handles normal items and also implements Conjured
    /// behavior (double degradation) by inspecting the item's name.
    /// </summary>
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
}
