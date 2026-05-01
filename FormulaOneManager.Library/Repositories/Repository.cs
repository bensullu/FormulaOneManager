// Abstract in-memory repository implementing every IRepository operation.
using FormulaOneManager.Library.Domain.Common;
using FormulaOneManager.Library.Exceptions;

namespace FormulaOneManager.Library.Repositories;

// Concrete repositories only need to inherit from this base in order to
// gain a fully working CRUD implementation. This is the "common code base"
// built on top of the abstraction layer required by the assignment.
public abstract class Repository<T> : IRepository<T> where T : IRacingEntity
{
    // Internal store using the entity Id as the key.
    private readonly Dictionary<Guid, T> _items = new();

    // Number of stored entities.
    public int Count => _items.Count;

    // Adds a new entity, throwing if an entity with the same Id already exists.
    public virtual void Add(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        // Guarantee the entity has an identifier before storing it.
        if (entity.Id == Guid.Empty)
            entity.Id = Guid.NewGuid();

        // Reject duplicates explicitly with a custom exception.
        if (_items.ContainsKey(entity.Id))
            throw new DuplicateEntityException(typeof(T), entity.Id);

        _items[entity.Id] = entity;
    }

    // Updates an existing entity; throws if it does not exist.
    public virtual void Update(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        // Reject updates of unknown entities.
        if (!_items.ContainsKey(entity.Id))
            throw new EntityNotFoundException(typeof(T), entity.Id);

        _items[entity.Id] = entity;
    }

    // Removes an entity by id, throwing when missing.
    public virtual void Remove(Guid id)
    {
        if (!_items.Remove(id))
            throw new EntityNotFoundException(typeof(T), id);
    }

    // Returns the entity with the given id, throws when missing.
    public T GetById(Guid id) =>
        FindById(id) ?? throw new EntityNotFoundException(typeof(T), id);

    // Returns the entity with the given id or null.
    public T? FindById(Guid id) =>
        _items.TryGetValue(id, out T? value) ? value : default;

    // Returns a snapshot of every entity. Returning an array prevents
    // callers from mutating the repository through the returned reference.
    public IReadOnlyList<T> GetAll() => _items.Values.ToArray();

    // First match for the predicate or null.
    public T? FirstOrDefault(Func<T, bool> predicate) =>
        _items.Values.FirstOrDefault(predicate);

    // Every match for the predicate.
    public IEnumerable<T> Where(Func<T, bool> predicate) =>
        _items.Values.Where(predicate);

    // Removes every stored entity.
    public void Clear() => _items.Clear();

    // Replaces the entire content with the supplied collection.
    // Used when restoring a snapshot from persisted JSON / XML.
    public void Reload(IEnumerable<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        Clear();
        foreach (T item in items)
        {
            // We bypass the duplicate check by writing directly so that
            // freshly loaded data is treated as authoritative.
            if (item.Id == Guid.Empty)
                item.Id = Guid.NewGuid();
            _items[item.Id] = item;
        }
    }
}
