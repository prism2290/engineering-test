using System;

namespace GildedRose.Console.Updaters
{
    // Utility helpers used by the concrete updaters.
    // - Centralizes quality bounds (Min/Max)
    // - Provides predicates to identify special items by name (name-based matching)
    // - Implements ChangeQuality that enforces clamping and ignores Sulfuras
    //
    // High-priority notes (read before changing matching rules):
    // - Name-based detection (e.g. matching "Conjured") is acceptable for
    //   this kata but is brittle; prefer canonical item types or metadata in
    //   production systems to avoid fragile string matching.
    // - Matching is case-insensitive and searches for the substring anywhere
    //   in the item name (uses OrdinalIgnoreCase). This is intentional but
    //   should be documented if item naming conventions change.
    // - Always use ItemUpdaterHelpers.ChangeQuality to adjust Quality so the
    //   0..50 invariant is enforced in one place; avoid direct mutations.
    internal static class ItemUpdaterHelpers
    {
        public const int MinQuality = 0;
        public const int MaxQuality = 50;

        public static bool IsSulfuras(Item item) =>
            item?.Name?.IndexOf("Sulfuras", StringComparison.OrdinalIgnoreCase) >= 0;

        public static bool IsAgedBrie(Item item) =>
            string.Equals(item?.Name, "Aged Brie", StringComparison.OrdinalIgnoreCase);

        public static bool IsBackstage(Item item) =>
            item?.Name?.IndexOf("Backstage", StringComparison.OrdinalIgnoreCase) >= 0;

        public static bool IsConjured(Item item) =>
            item?.Name?.IndexOf("Conjured", StringComparison.OrdinalIgnoreCase) >= 0;

        public static void ChangeQuality(Item item, int delta)
        {
            if (item == null) return;
            if (IsSulfuras(item)) return; // legendary item does not change

            var q = item.Quality + delta;
            if (q < MinQuality) q = MinQuality;
            if (q > MaxQuality) q = MaxQuality;
            item.Quality = q;
        }
    }
}
