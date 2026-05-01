// A Formula 1 team that can sign drivers under contract.
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using FormulaOneManager.Library.Domain.Common;

namespace FormulaOneManager.Library.Domain.Teams;

// Concrete entity inheriting from the abstract RacingEntity layer.
public class Team : RacingEntity
{
    // Official team name (e.g. "Mercedes-AMG Petronas F1 Team").
    public string TeamName { get; set; } = string.Empty;

    // Headquarters location (e.g. "Brackley, UK").
    public string Headquarters { get; set; } = string.Empty;

    // Country in which the team is registered.
    public string Country { get; set; } = string.Empty;

    // Annual budget cap (in millions of USD).
    public double BudgetMillions { get; set; }

    // Year the team was founded.
    public int FoundedYear { get; set; }

    // Display name used by the GUI lists.
    [JsonIgnore]
    [XmlIgnore]
    public override string DisplayName =>
        string.IsNullOrWhiteSpace(TeamName) ? "(unnamed team)" : TeamName;

    // Default string representation used by the data grid.
    public override string ToString() => $"{TeamName} ({Headquarters})";
}
