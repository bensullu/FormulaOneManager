// Common base interface for every persisted racing entity.
namespace FormulaOneManager.Library.Domain.Common;

// Every entity in the system must expose a unique identifier so that
// generic repositories can locate, update or remove it.
public interface IRacingEntity
{
    // Globally unique identifier for the entity.
    Guid Id { get; set; }

    // Short human readable label used in lists and logs.
    string DisplayName { get; }
}
