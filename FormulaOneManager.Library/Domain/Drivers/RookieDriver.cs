// Concrete Driver subclass representing a rookie / test driver.
namespace FormulaOneManager.Library.Domain.Drivers;

// Rookie drivers haven't yet committed to a category and act as testers.
public class RookieDriver : Driver
{
    // Total testing kilometers logged so far.
    public double TestingKilometers { get; set; }

    // Hard coded category name.
    [System.Text.Json.Serialization.JsonIgnore]
    [System.Xml.Serialization.XmlIgnore]
    public override string Category => "Rookie";

    // Rookies take it one lap at a time.
    public override string GetRacingMotto() => "Every lap is a lesson.";
}
