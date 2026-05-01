// Abstract definition of a racing driver in the system.
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using FormulaOneManager.Library.Domain.Common;

namespace FormulaOneManager.Library.Domain.Drivers;

// Polymorphic JSON metadata: each derived type is registered with a discriminator
// so System.Text.Json can serialize and deserialize the proper concrete type.
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$category")]
[JsonDerivedType(typeof(F1Driver), typeDiscriminator: "f1")]
[JsonDerivedType(typeof(F2Driver), typeDiscriminator: "f2")]
[JsonDerivedType(typeof(KartingDriver), typeDiscriminator: "karting")]
[JsonDerivedType(typeof(RookieDriver), typeDiscriminator: "rookie")]
// XmlInclude attributes register every concrete subtype with the XmlSerializer.
[XmlInclude(typeof(F1Driver))]
[XmlInclude(typeof(F2Driver))]
[XmlInclude(typeof(KartingDriver))]
[XmlInclude(typeof(RookieDriver))]
public abstract class Driver : RacingEntity
{
    // First name of the driver.
    public string FirstName { get; set; } = string.Empty;

    // Last name of the driver.
    public string LastName { get; set; } = string.Empty;

    // Country of origin (e.g. "United Kingdom", "Netherlands").
    public string Nationality { get; set; } = string.Empty;

    // Age of the driver in years.
    public int Age { get; set; }

    // Gender of the driver.
    public DriverGender Gender { get; set; } = DriverGender.Unknown;

    // Current fitness status.
    public FitnessStatus Fitness { get; set; } = FitnessStatus.Unknown;

    // Date the driver registered with our management system.
    public DateTime RegistrationDate { get; set; } = DateTime.Today;

    // Whether the driver currently has an active contract with a team.
    public bool HasContract { get; set; }

    // The category name implemented by every concrete subclass.
    [JsonIgnore]
    [XmlIgnore]
    public abstract string Category { get; }

    // Each subclass returns its racing motto (used in tooltips and logs).
    public abstract string GetRacingMotto();

    // Returns null when the driver is available to sign, otherwise a sentence
    // describing the first business rule that prevents the contract.
    // Concrete subclasses may chain to base.GetAvailabilityIssue() and add
    // category specific rules (minimum / maximum age etc.).
    public virtual string? GetAvailabilityIssue()
    {
        // Reject drivers who already have an active contract.
        if (HasContract)
            return "Driver already has an active contract with another team.";

        // Translate the fitness status into a friendly explanation.
        return Fitness switch
        {
            FitnessStatus.Fit => null,
            FitnessStatus.Injured => "Driver is currently injured and cannot race.",
            FitnessStatus.Suspended => "Driver is suspended by the FIA.",
            FitnessStatus.Retired => "Driver has retired from competition.",
            FitnessStatus.Unknown => "Driver has not yet passed the medical examination.",
            _ => "Driver is not in fit racing condition."
        };
    }

    // Convenience predicate that simply checks for the absence of any issue.
    // Subclasses should override GetAvailabilityIssue rather than this method
    // so the textual reason stays in sync.
    public bool IsAvailable() => GetAvailabilityIssue() is null;

    // The display name combines the category and the driver's full name.
    [JsonIgnore]
    [XmlIgnore]
    public override string DisplayName =>
        $"{Category}: {FirstName} {LastName}".Trim();

    // Helpful textual representation used by the GUI grids.
    public override string ToString() =>
        $"{DisplayName} ({Nationality}, {Age}y, {Fitness})";
}
