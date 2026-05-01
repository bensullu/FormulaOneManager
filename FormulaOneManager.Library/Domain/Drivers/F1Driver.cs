// Concrete Driver subclass representing a Formula 1 driver.
using FormulaOneManager.Library.Exceptions;

namespace FormulaOneManager.Library.Domain.Drivers;

// F1 drivers track championship titles and pole positions for statistics.
public class F1Driver : Driver
{
    // Number of World Drivers' Championship titles already won.
    public int ChampionshipTitles { get; set; }

    // Total career pole positions.
    public int PolePositions { get; set; }

    // True when the driver is currently a championship contender.
    public bool IsChampionshipContender { get; set; } = true;

    // Hard coded category name used for filtering and display.
    [System.Text.Json.Serialization.JsonIgnore]
    [System.Xml.Serialization.XmlIgnore]
    public override string Category => "F1";

    // Returns the canonical motto used in the UI tooltips.
    public override string GetRacingMotto() => "Lights out and away we go!";

    // Adds the FIA Super Licence age requirement on top of the base rules.
    // FIA regulations require an F1 driver to be at least 18 years old.
    public override string? GetAvailabilityIssue()
    {
        // Run the shared rules first (contract / fitness checks).
        string? baseIssue = base.GetAvailabilityIssue();
        if (baseIssue != null) return baseIssue;

        // Reject under-age F1 drivers explicitly.
        if (Age < 18)
            return "Driver is below the FIA Super Licence minimum age of 18 for Formula 1.";

        return null;
    }

    // Hard rule enforced when adding or updating: F1 drivers must be
    // at least 18 years old (FIA Super Licence). Throws a ValidationException
    // so the GUI can surface the reason directly.
    public override void EnsureValidForCategory()
    {
        if (Age < 18)
            throw new ValidationException(nameof(Age),
                "Formula 1 drivers must be at least 18 years old (FIA Super Licence rule).");
    }
}
