// Extension methods that work on top of any IRepository or IEnumerable.
using FormulaOneManager.Library.Domain.Common;
using FormulaOneManager.Library.Domain.Drivers;
using FormulaOneManager.Library.Repositories;

namespace FormulaOneManager.Library.Extensions;

// All methods are generic / work on the abstraction layer so they can be
// reused with any concrete entity that the user later adds to the system.
public static class RacingExtensions
{
    // Returns the entities sorted by their display name using LINQ.
    public static IEnumerable<T> OrderByName<T>(this IEnumerable<T> source)
        where T : IRacingEntity =>
        source.OrderBy(x => x.DisplayName, StringComparer.OrdinalIgnoreCase);

    // Returns a friendly summary of how many entities match a predicate.
    public static string CountAndDescribe<T>(
        this IEnumerable<T> source,
        Func<T, bool> predicate,
        string singular,
        string plural)
        where T : IRacingEntity
    {
        // Counting once via LINQ.
        int count = source.Count(predicate);

        // Building the localized message manually so the wording stays in English.
        return count == 1 ? $"1 {singular}" : $"{count} {plural}";
    }

    // Returns drivers that are currently available to sign (LINQ filter).
    public static IEnumerable<Driver> AvailableOnly(this IEnumerable<Driver> source) =>
        source.Where(d => d.IsAvailable());

    // Returns drivers that match the provided category name (case insensitive).
    public static IEnumerable<Driver> ByCategory(
        this IEnumerable<Driver> source, string category) =>
        source.Where(d => string.Equals(d.Category, category, StringComparison.OrdinalIgnoreCase));

    // Generic helper that adds many entities to a repository at once.
    // Demonstrates a generic extension method on top of the abstraction.
    public static void AddRange<T>(this IRepository<T> repository, IEnumerable<T> items)
        where T : IRacingEntity
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(items);

        foreach (T item in items)
            repository.Add(item);
    }
}
