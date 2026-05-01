// Generic repository contract for any entity that owns a Guid identifier.
using FormulaOneManager.Library.Domain.Common;

namespace FormulaOneManager.Library.Repositories;

// The constraint guarantees the entity owns an Id we can use as a key.
// Returning IReadOnlyList lets callers use LINQ on the result without
// being able to mutate the underlying repository state directly.
public interface IRepository<T> where T : IRacingEntity
{
    // Adds a brand new entity to the repository.
    void Add(T entity);

    // Updates an existing entity (matched by Id).
    void Update(T entity);

    // Removes an entity by its identifier; throws when not found.
    void Remove(Guid id);

    // Returns the entity with the given identifier or throws when not found.
    T GetById(Guid id);

    // Tries to return the entity with the given identifier (returns null when missing).
    T? FindById(Guid id);

    // Snapshot of every entity currently stored, in arbitrary order.
    IReadOnlyList<T> GetAll();

    // Returns the first entity matching the predicate or null.
    T? FirstOrDefault(Func<T, bool> predicate);

    // Returns every entity matching the predicate.
    IEnumerable<T> Where(Func<T, bool> predicate);

    // Number of stored entities (cheap O(1) accessor).
    int Count { get; }

    // Clears every entity. Used when loading data from disk.
    void Clear();
}
