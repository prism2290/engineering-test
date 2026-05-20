namespace GildedRose.Console.Updaters
{
    /// <summary>
    /// Strategy interface for per-item update logic.
    /// Implementations handle specific item types (Aged Brie, Backstage, etc.)
    /// and apply the business rules to adjust <see cref="Item"/> properties.
    /// </summary>
    public interface IItemUpdater
    {
        bool CanHandle(Item item);
        void Update(Item item);
    }
}
