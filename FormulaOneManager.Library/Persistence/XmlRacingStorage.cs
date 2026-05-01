// XML based implementation of the persistence abstraction.
using System.Xml.Serialization;
using FormulaOneManager.Library.Domain.Drivers;
using FormulaOneManager.Library.Exceptions;

namespace FormulaOneManager.Library.Persistence;

// XmlSerializer needs to know about every concrete driver subtype so it can
// round-trip the polymorphic Driver list. This second concrete persistence
// class also proves the IRacingStorage abstraction supports more than one format.
public class XmlRacingStorage : IRacingStorage
{
    // File extension shown in the file dialogs.
    public string FileExtension => ".xml";

    // Friendly description shown in the dialog filter.
    public string Description => "Racing XML snapshot (*.xml)";

    // Loads a snapshot from an XML file.
    public RacingSnapshot Load(string path)
    {
        try
        {
            XmlSerializer serializer = new(typeof(RacingSnapshot), GetExtraTypes());
            using FileStream stream = File.OpenRead(path);
            return (RacingSnapshot?)serializer.Deserialize(stream) ?? new RacingSnapshot();
        }
        catch (Exception ex) when (ex is not RacingException)
        {
            throw new RacingException($"Failed to load XML snapshot from '{path}'.", ex);
        }
    }

    // Saves a snapshot to an XML file.
    public void Save(string path, RacingSnapshot snapshot)
    {
        try
        {
            XmlSerializer serializer = new(typeof(RacingSnapshot), GetExtraTypes());
            using FileStream stream = File.Create(path);
            serializer.Serialize(stream, snapshot);
        }
        catch (Exception ex) when (ex is not RacingException)
        {
            throw new RacingException($"Failed to save XML snapshot to '{path}'.", ex);
        }
    }

    // Extra types XmlSerializer must know about to support polymorphism.
    private static Type[] GetExtraTypes() => new[]
    {
        typeof(F1Driver),
        typeof(F2Driver),
        typeof(KartingDriver),
        typeof(RookieDriver)
    };
}
