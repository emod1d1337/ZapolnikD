namespace ClinicCRM;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Database.Initialize();
        Application.Run(new MainForm());
    }
}
