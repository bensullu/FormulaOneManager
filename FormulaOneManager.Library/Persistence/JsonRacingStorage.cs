// JSON implementation of the persistence abstraction.
using System.Text.Json;
using System.Text.Json.Serialization;
using FormulaOneManager.Library.Exceptions;

namespace FormulaOneManager.Library.Persistence;

// Uses System.Text.Json with polymorphic support so the abstract Driver
// type is round-tripped correctly thanks to the [JsonDerivedType] attributes
// declared on the Driver class.
public class JsonRacingStorage : IRacingStorage
{
    // Serializer settings tuned for human readability.
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    // File extension exposed to the file dialogs.
    public string FileExtension => ".json";

    // Friendly description shown in the file dialog filter.
    public string Description => "Racing JSON snapshot (*.json)";

    // Loads a snapshot from disk, wrapping IO failures in our exception.
    public RacingSnapshot Load(string path)
    {
        try
        {
            using FileStream stream = File.OpenRead(path);
            return JsonSerializer.Deserialize<RacingSnapshot>(stream, Options)
                   ?? new RacingSnapshot();
        }
        catch (Exception ex) when (ex is not RacingException)
        {
            // Wrap unknown errors so the GUI can show a single friendly message.
            throw new RacingException($"Failed to load JSON snapshot from '{path}'.", ex);
        }
    }

    // Saves the supplied snapshot to disk.
    public void Save(string path, RacingSnapshot snapshot)
    {
        try
        {
            using FileStream stream = File.Create(path);
            JsonSerializer.Serialize(stream, snapshot, Options);
        }
        catch (Exception ex) when (ex is not RacingException)
        {
            throw new RacingException($"Failed to save JSON snapshot to '{path}'.", ex);
        }
    }
}
