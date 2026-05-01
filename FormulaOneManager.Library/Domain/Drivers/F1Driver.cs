// Concrete Driver subclass representing a Formula 1 driver.
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
}
