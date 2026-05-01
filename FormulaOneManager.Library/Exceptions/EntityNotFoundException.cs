// Thrown when a repository cannot locate an entity by its identifier.
namespace FormulaOneManager.Library.Exceptions;

// Carries the entity type and id that were searched for.
public class EntityNotFoundException : RacingException
{
    // Type of entity that was not found.
    public Type EntityType { get; }

    // Identifier that could not be located.
    public Guid EntityId { get; }

    // Builds an informative message including the missing id.
    public EntityNotFoundException(Type entityType, Guid id)
        : base($"Entity of type '{entityType.Name}' with id '{id}' was not found.")
    {
        EntityType = entityType;
        EntityId = id;
    }
}
