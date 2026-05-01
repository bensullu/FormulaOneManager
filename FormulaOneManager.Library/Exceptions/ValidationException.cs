// Thrown when a user-provided value is rejected by a validator.
namespace FormulaOneManager.Library.Exceptions;

// Used from the GUI to show friendly inline error messages.
public class ValidationException : RacingException
{
    // Name of the field that failed validation.
    public string FieldName { get; }

    // Standard constructor combining the field and the issue.
    public ValidationException(string fieldName, string message)
        : base($"Invalid value for '{fieldName}': {message}")
    {
        FieldName = fieldName;
    }
}
