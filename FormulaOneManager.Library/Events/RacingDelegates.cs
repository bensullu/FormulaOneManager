// Delegates and EventArgs subclasses used by the racing service.
using FormulaOneManager.Library.Domain.Contracts;
using FormulaOneManager.Library.Domain.Drivers;

namespace FormulaOneManager.Library.Events;

// Custom delegate signaling that a driver was added to the system.
public delegate void DriverAddedHandler(object? sender, DriverEventArgs e);

// Custom delegate signaling that a driver was successfully signed.
public delegate void DriverSignedHandler(object? sender, ContractEventArgs e);

// Custom delegate used as a generic logger across the library.
public delegate void RacingLogHandler(string message);

// EventArgs subclass carrying a reference to the affected driver.
public class DriverEventArgs : EventArgs
{
    // The driver involved in the event.
    public Driver Driver { get; }

    // Constructor saving the supplied driver reference.
    public DriverEventArgs(Driver driver) => Driver = driver;
}

// EventArgs subclass carrying the full contract record.
public class ContractEventArgs : EventArgs
{
    // The freshly created contract record.
    public Contract Contract { get; }

    // The signed driver as a convenience for the GUI.
    public Driver Driver { get; }

    // Constructor saving both references.
    public ContractEventArgs(Contract contract, Driver driver)
    {
        Contract = contract;
        Driver = driver;
    }
}
