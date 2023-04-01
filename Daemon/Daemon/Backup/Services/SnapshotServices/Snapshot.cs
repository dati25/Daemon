namespace Daemon.Backup.Services.SnapshotServices;
public class Snapshot
{
    public string Path { get; set; }
    public DateTime LastModified { get; set; }

    public Snapshot(string path, DateTime lastModified)
    {
        Path = path;
        LastModified = lastModified;
    }
}