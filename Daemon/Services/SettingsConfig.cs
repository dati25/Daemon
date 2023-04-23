namespace Daemon.Services;
public class SettingsConfig
{
    public readonly string SettingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup");
    public readonly string PcPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup", "pc.json");
    public readonly string ConfigsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup", "configs.json");
    public readonly string SnapshotsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup", "Snapshots");
}