namespace Daemon.Models;
public class Snapshot
{
    public int? ConfigId { get; set; }
    public string Path { get; set; }
    public DateTime LastModified { get; set; }

    public Snapshot(string path, DateTime lastModified)
    {
        this.Path = path;
        this.LastModified = lastModified;
    }
}