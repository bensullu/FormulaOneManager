// Concrete Driver subclass representing a karting driver.
namespace FormulaOneManager.Library.Domain.Drivers;

// Karting drivers track their racing class and recent victories.
public class KartingDriver : Driver
{
    // Karting class such as "KZ", "OK-Junior", "Mini60".
    public string KartingClass { get; set; } = "OK-Junior";

    // Number of victories in the current season.
    public int SeasonWins { get; set; }

    // Hard coded category name.
    [System.Text.Json.Serialization.JsonIgnore]
    [System.Xml.Serialization.XmlIgnore]
    public override string Category => "Karting";

    // Karting is where champions are born.
    public override string GetRacingMotto() => "Future world champion in the making!";

    // Karting drivers must be young enough to qualify for junior series.
    public override bool IsAvailable() =>
        base.IsAvailable() && Age <= 18;
}
