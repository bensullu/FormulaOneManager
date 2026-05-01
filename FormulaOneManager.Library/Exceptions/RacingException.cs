// Base class for every custom exception thrown by the library.
namespace FormulaOneManager.Library.Exceptions;

// Subclassing a single root makes it easy for the GUI to catch all of
// our domain errors with one catch block while still letting unrelated
// runtime exceptions bubble up.
public class RacingException : Exception
{
    // Default constructor with a generic message.
    public RacingException() : base("A racing operation failed.") { }

    // Standard constructor accepting a human readable message.
    public RacingException(string message) : base(message) { }

    // Constructor used when wrapping another exception (e.g. file IO error).
    public RacingException(string message, Exception inner) : base(message, inner) { }
}
