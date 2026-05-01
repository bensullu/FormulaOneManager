// Plain DTO holding the full state of the management system for serialization.
using FormulaOneManager.Library.Domain.Contracts;
using FormulaOneManager.Library.Domain.Drivers;
using FormulaOneManager.Library.Domain.Teams;

namespace FormulaOneManager.Library.Persistence;

// Using a snapshot keeps the serialized format independent of the runtime
// repository internals - the Driver polymorphism is preserved through the
// JsonDerivedType attributes on the Driver class.
public class RacingSnapshot
{
    // Schema version, useful when we eventually need migrations.
    public int Version { get; set; } = 1;

    // The list of drivers at the time of the snapshot.
    public List<Driver> Drivers { get; set; } = new();

    // The list of registered teams.
    public List<Team> Teams { get; set; } = new();

    // The list of recorded contracts.
    public List<Contract> Contracts { get; set; } = new();

    // Date and time the snapshot was created.
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
