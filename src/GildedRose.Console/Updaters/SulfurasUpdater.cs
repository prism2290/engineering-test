namespace GildedRose.Console.Updaters
{
    /// <summary>
    /// Updater for the legendary "Sulfuras" item. This is a no-op because
    /// Sulfuras never changes in Quality or SellIn.
    /// </summary>
    public class SulfurasUpdater : IItemUpdater
    {
        public bool CanHandle(Item item) => ItemUpdaterHelpers.IsSulfuras(item);
        public void Update(Item item) { /* legendary item - no changes */ }
    }
}
