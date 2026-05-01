// Entry point of the Formula 1 Manager desktop application.
namespace FormulaOneManager.App;

// Standard WinForms bootstrap class.
internal static class Program
{
    // The Main method is marked STAThread because WinForms requires it.
    [STAThread]
    private static void Main()
    {
        // Configure WinForms with default high DPI and font settings.
        ApplicationConfiguration.Initialize();

        // Launch the main window.
        Application.Run(new MainForm());
    }
}
