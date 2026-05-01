// Concrete Driver subclass representing a Formula 2 driver.
namespace FormulaOneManager.Library.Domain.Drivers;

// F2 drivers expose feeder series specific attributes.
public class F2Driver : Driver
{
    // True when the driver is part of an F1 team's junior academy.
    public bool IsJuniorAcademyMember { get; set; } = true;

    // Current championship standing (1 = leader, higher = lower).
    public int CurrentStanding { get; set; } = 10;

    // Hard coded category name.
    [System.Text.Json.Serialization.JsonIgnore]
    [System.Xml.Serialization.XmlIgnore]
    public override string Category => "F2";

    // F2 drivers are hungry for promotion.
    public override string GetRacingMotto() => "On the road to F1!";
}
