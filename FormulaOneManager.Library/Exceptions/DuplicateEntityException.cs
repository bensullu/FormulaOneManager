// Thrown when adding an entity whose identifier already exists.
namespace FormulaOneManager.Library.Exceptions;

// Mostly defensive - identifiers are auto generated, but we still want a
// dedicated exception for any future code path that imports external data.
public class DuplicateEntityException : RacingException
{
    // Type of entity that was duplicated.
    public Type EntityType { get; }

    // The conflicting identifier.
    public Guid EntityId { get; }

    // Standard constructor producing a descriptive message.
    public DuplicateEntityException(Type entityType, Guid id)
        : base($"An entity of type '{entityType.Name}' with id '{id}' already exists.")
    {
        EntityType = entityType;
        EntityId = id;
    }
}
