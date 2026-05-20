# Changelog

All notable changes made to this repository are documented in this file.

## 2026-05-20 — Refactor & Conjured Support (multiple logical commits)

### Add DI package
- Files: `src/GildedRose.Console/GildedRose.Console.csproj`
- What: Added `Microsoft.Extensions.DependencyInjection` package reference.
- Decision / trade-off: Chose the lightweight, well-known Microsoft DI package to enable constructor injection without bringing in the full generic host. Adds a small dependency but improves testability and extensibility.
- Alternatives: Keep manual static registration (no dependency), or use `Host.CreateDefaultBuilder` (full hosting stack).

### Wire `Program` to use DI
- Files: `src/GildedRose.Console/Program.cs`
- What: Replaced the static updater list with constructor-injected `IEnumerable<IItemUpdater>` and registered concrete updaters in `Main` using `ServiceCollection`.
- Decision / trade-off: Manual registration preserves explicit ordering (priority) and keeps startup simple; constructor injection improves testability. Slightly more code in `Main` but clearer DI contract.
- Alternatives: Assembly-scanning auto-registration, or keep the original static list.

### Extract Strategy Updaters
- Files: `src/GildedRose.Console/Updaters/*`, `src/GildedRose.Console/Interfaces/IItemUpdater.cs`
- What: Refactored per-item update logic into single-responsibility updater classes (AgedBrieUpdater, BackstageUpdater, SulfurasUpdater, DefaultUpdater) implementing `IItemUpdater` (Strategy pattern).
- Decision / trade-off: Improves maintainability and unit testability by separating concerns. Adds a few small classes and an interface, increasing the number of files but making responsibilities explicit.
- Alternatives: Use polymorphic `Item` subclasses (would require changing `Item`), or keep a single large `UpdateQuality` with branching logic (simpler but harder to extend).

### Centralize helper functions
- Files: `src/GildedRose.Console/Helpers/ItemUpdaterHelpers.cs`
- What: Centralized quality bounds, name-based predicates, and `ChangeQuality` with clamping logic.
- Decision / trade-off: Central helper prevents duplicated clamping logic and enforces invariants in one place. Kept name-based matching for simplicity (suitable for kata), but noted brittleness.
- Alternatives: Add explicit item metadata or enum to avoid string matching.

### Implement Conjured behavior
- Files: `src/GildedRose.Console/Updaters/DefaultUpdater.cs`
- What: `DefaultUpdater` detects `Conjured` via name substring and degrades quality twice as fast (and twice again after sell date), satisfying the kata requirement.
- Decision / trade-off: Implemented within `DefaultUpdater` to minimize added code; name-based detection is quick but brittle.
- Alternatives: Add a dedicated `ConjuredUpdater` class or tag items with a type property.

### Add focused unit tests
- Files: `src/GildedRose.Tests/*`
- What: Added tests for Aged Brie, Backstage, Sulfuras, Default, Conjured behaviors and a constructor-injection test for `Program`.
- Decision / trade-off: Tests are focused and instantiate updaters directly to keep unit tests fast and isolated. Also added a helper to centralize updater ordering in tests.
- Alternatives: Add integration tests that resolve `Program` from a `ServiceProvider` or add property-based tests for broader coverage.

### Test helper and documentation improvements
- Files: `src/GildedRose.Tests/TestHelpers/UpdaterTestHelper.cs`, comments across codebase
- What: Added `UpdaterTestHelper` to provide the default ordered updaters for tests; added XML docs and clarifying comments describing DI contract, matching semantics, and helper usage.
- Decision / trade-off: Small, low-risk improvements to developer experience and maintainability. No behavioral changes.

### Additional test: Conjured after sell date
- Files: `src/GildedRose.Tests/ItemUpdaterFocusedTests.cs`
- What: Added a test asserting a Conjured item with `SellIn = 0` loses 4 Quality in one update (double + post-sell double), preventing regressions for combined behavior.

## Summary
- Status: All existing rules from the kata are preserved and Conjured behavior is implemented; unit tests pass locally.
- Primary design directions: Improved maintainability and testability via Strategy pattern and constructor injection; favored explicit manual registrations for predictable ordering.
- Noted limitations: Name-based item detection is brittle; consider explicit item-type metadata for production scenarios.

If you want, I can convert these logical entries into formal git commit messages, add a `Unreleased` section at the top, or adjust the wording for an upstream CHANGELOG convention.
