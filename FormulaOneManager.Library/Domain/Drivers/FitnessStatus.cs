// Enumeration describing the current racing fitness of a driver.
namespace FormulaOneManager.Library.Domain.Drivers;

// Used both for filtering in LINQ queries and for display in the GUI.
public enum FitnessStatus
{
    // The driver has not been medically examined yet.
    Unknown = 0,

    // The driver is fit and ready to race.
    Fit = 1,

    // The driver is currently injured and cannot race.
    Injured = 2,

    // The driver is suspended by the FIA.
    Suspended = 3,

    // The driver retired from active competition.
    Retired = 4
}
