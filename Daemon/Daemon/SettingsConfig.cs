namespace Daemon
{
    public class SettingsConfig
    {
        public string SETTINGSDIR = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup");
        public string PCPATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup", "pc.json");
        public string CONFIGSPATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup", "configs.json");
        public string SNAPSHOTSPATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup", "snapshots.json");
    }
}
