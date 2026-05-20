using System;
using System.Collections.Generic;

namespace GildedRose.Console;

public class Program
{
    public IList<Item> Items = new List<Item>();

    static void Main(string[] args)
    {
        System.Console.WriteLine("OMGHAI!");

        var app = new Program()
                      {
                          Items = new List<Item>
                                      {
                                          new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
                                          new Item {Name = "Aged Brie", SellIn = 2, Quality = 0},
                                          new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
                                          new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
                                          new Item
                                              {
                                                  Name = "Backstage passes to a TAFKAL80ETC concert",
                                                  SellIn = 15,
                                                  Quality = 20
                                              },
                                          new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
                                      }

                      };

        app.UpdateQuality();

        System.Console.ReadKey();
    }

    public void UpdateQuality()
    {
        /*
         * Refactor notes:
         * - Extracted small helper predicates to identify item types.
         * - Centralized min/max quality constants and a ChangeQuality helper to clamp quality.
         * - Added Conjured detection: items with "Conjured" in the name degrade twice as fast.
         * - Flattened the original nested logic into clearer per-item steps to ease maintenance
         *   and to prepare for a future strategy-based extraction.
         */
        const int MinQuality = 0;
        const int MaxQuality = 50;

        // Predicates to identify special item behaviors.
        bool IsAgedBrie(Item it) => it.Name == "Aged Brie";
        bool IsBackstage(Item it) => it.Name == "Backstage passes to a TAFKAL80ETC concert";
        bool IsSulfuras(Item it) => it.Name == "Sulfuras, Hand of Ragnaros";
        // Conjured items degrade twice as fast; detected by name substring (case-insensitive).
        bool IsConjured(Item it) => !string.IsNullOrEmpty(it.Name) && it.Name.IndexOf("Conjured", StringComparison.OrdinalIgnoreCase) >= 0;

        // Helper that changes quality while enforcing min/max bounds and skipping Sulfuras.
        void ChangeQuality(Item it, int delta)
        {
            if (IsSulfuras(it)) return; // Legendary item; quality never changes
            var q = it.Quality + delta;
            if (q < MinQuality) q = MinQuality;
            if (q > MaxQuality) q = MaxQuality;
            it.Quality = q;
        }

        // Loop through items and apply rules in a single-pass, readable sequence.
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            // Conjured items degrade twice as fast (decrement value applied per step).
            var decrement = IsConjured(item) ? 2 : 1;

            // Pre sell-date behavior
            if (IsAgedBrie(item))
            {
                // Aged Brie increases in quality as it gets older.
                ChangeQuality(item, +1);
            }
            else if (IsBackstage(item))
            {
                // Backstage passes increase as the concert approaches.
                ChangeQuality(item, +1);
                if (item.SellIn < 11) ChangeQuality(item, +1);
                if (item.SellIn < 6) ChangeQuality(item, +1);
            }
            else
            {
                // Default items (including Conjured) degrade in quality.
                ChangeQuality(item, -decrement);
            }

            // Decrease sell-in for non-legendary items
            if (!IsSulfuras(item)) item.SellIn -= 1;

            // Post sell-date behavior: additional changes after SellIn < 0
            if (item.SellIn < 0)
            {
                if (IsAgedBrie(item))
                {
                    // Aged Brie increases again after the sell date.
                    ChangeQuality(item, +1);
                }
                else if (IsBackstage(item))
                {
                    // Backstage passes drop to 0 after the concert.
                    item.Quality = 0;
                }
                else
                {
                    // Default (and Conjured) items degrade again after sell date.
                    ChangeQuality(item, -decrement);
                }
            }
        }
    }
}

public class Item
{
    public string Name { get; set; } = "";

    public int SellIn { get; set; }

    public int Quality { get; set; }
}
