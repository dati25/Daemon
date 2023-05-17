namespace Daemon.Services;
public static class SettingsConfig
{
    public readonly static string SettingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup");
    public readonly static string PcPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup", "pc.json");
    public readonly static string ConfigsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup", "configs.json");
    public readonly static string SnapshotsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup", "Snapshots");
}