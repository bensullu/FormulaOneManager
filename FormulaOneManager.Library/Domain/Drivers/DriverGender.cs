// Enumeration describing the gender of a racing driver.
namespace FormulaOneManager.Library.Domain.Drivers;

// Kept simple on purpose so the UI can bind it directly to a ComboBox.
public enum DriverGender
{
    // Unknown gender, used as a safe default for newly imported records.
    Unknown = 0,

    // Male driver.
    Male = 1,

    // Female driver.
    Female = 2
}
