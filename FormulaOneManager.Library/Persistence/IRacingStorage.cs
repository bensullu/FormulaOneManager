// Abstraction layer for persistence implementations.
namespace FormulaOneManager.Library.Persistence;

// Concrete implementations such as the JSON backed storage class
// inherit from this interface to plug seamlessly into the GUI.
public interface IRacingStorage
{
    // Reads a snapshot from the given file path.
    RacingSnapshot Load(string path);

    // Writes the supplied snapshot to the given file path.
    void Save(string path, RacingSnapshot snapshot);

    // File extension used by this storage in the open/save dialogs.
    string FileExtension { get; }

    // Friendly description used for the file dialog filter.
    string Description { get; }
}
