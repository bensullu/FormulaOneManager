// Thrown when a business rule prevents a contract from being signed.
namespace FormulaOneManager.Library.Exceptions;

// Examples include signing a driver who already has a contract, who is
// injured, or who is suspended by the FIA.
public class InvalidContractException : RacingException
{
    // Default error message.
    public InvalidContractException() : base("The contract cannot be signed.") { }

    // Custom error message constructor.
    public InvalidContractException(string message) : base(message) { }
}
