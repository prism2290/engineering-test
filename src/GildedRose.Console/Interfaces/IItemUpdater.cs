namespace GildedRose.Console.Updaters
{
    /// <summary>
    /// Strategy interface for per-item update logic.
    /// Implementations handle specific item types (Aged Brie, Backstage, etc.)
    /// and apply the business rules to adjust <see cref="Item"/> properties.
    /// </summary>
    /// <remarks>
    /// Implementations should be small, focused, and safe to call repeatedly
    /// for different items. Prefer pure behavior without hidden state so tests
    /// and DI resolution remain predictable.
    /// </remarks>
    public interface IItemUpdater
    {
        /// <summary>
        /// Returns <c>true</c> when this updater should be used for the given <paramref name="item"/>.
        /// The dispatcher selects the first updater that returns <c>true</c>.
        /// </summary>
        bool CanHandle(Item item);

        /// <summary>
        /// Apply a single update cycle to <paramref name="item"/> (adjust <see cref="Item.SellIn"/> and <see cref="Item.Quality"/>).
        /// </summary>
        void Update(Item item);
    }
}
