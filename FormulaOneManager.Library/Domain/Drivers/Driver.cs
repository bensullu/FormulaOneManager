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

    // Returns true when the driver is medically and contractually ready to sign.
    // The base implementation can be overridden if a category needs extra rules.
    public virtual bool IsAvailable() =>
        !HasContract && Fitness == FitnessStatus.Fit;

    // The display name combines the category and the driver's full name.
    [JsonIgnore]
    [XmlIgnore]
    public override string DisplayName =>
        $"{Category}: {FirstName} {LastName}".Trim();

    // Helpful textual representation used by the GUI grids.
    public override string ToString() =>
        $"{DisplayName} ({Nationality}, {Age}y, {Fitness})";
}
