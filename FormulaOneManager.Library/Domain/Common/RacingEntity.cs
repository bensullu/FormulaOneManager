// Abstract base class providing the identity field for every entity.
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace FormulaOneManager.Library.Domain.Common;

// First abstraction layer of the domain: forces every entity to have an Id
// and to expose a DisplayName used by the UI.
public abstract class RacingEntity : IRacingEntity
{
    // Stable identifier generated automatically when the entity is created.
    public Guid Id { get; set; } = Guid.NewGuid();

    // The concrete entity decides how to compose its display name.
    // Excluded from serialization because it is computed at runtime.
    [JsonIgnore]
    [XmlIgnore]
    public abstract string DisplayName { get; }
}
