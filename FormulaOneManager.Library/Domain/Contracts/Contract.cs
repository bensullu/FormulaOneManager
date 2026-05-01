// Records the act of a team signing a driver to race for them.
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using FormulaOneManager.Library.Domain.Common;

namespace FormulaOneManager.Library.Domain.Contracts;

// A Contract ties together one Driver and one Team for a season range.
public class Contract : RacingEntity
{
    // Identifier of the driver who has been signed.
    public Guid DriverId { get; set; }

    // Identifier of the team that signed the driver.
    public Guid TeamId { get; set; }

    // The day the contract was signed.
    public DateTime SignedDate { get; set; } = DateTime.Today;

    // Length of the contract in seasons (1, 2, 3, ...).
    public int DurationSeasons { get; set; } = 1;

    // Total contract value in millions of USD.
    public double ValueMillions { get; set; }

    // Optional textual notes from the team principal.
    public string Notes { get; set; } = string.Empty;

    // Display name based on the Id - the GUI replaces this with rich labels.
    [JsonIgnore]
    [XmlIgnore]
    public override string DisplayName => $"Contract {Id:N}".Substring(0, 16);
}
