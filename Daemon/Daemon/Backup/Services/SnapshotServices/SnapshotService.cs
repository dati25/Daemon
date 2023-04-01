namespace Daemon.Backup.Services.SnapshotServices;

public class SnapshotService
{
    public List<Snapshot> ReadSnapshot(string snapshotPath)
    {
        List<Snapshot> snaps = new List<Snapshot>();

        using (StreamReader sr = new StreamReader(snapshotPath))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] strings = line.Split('|');
                snaps.Add(new Snapshot(strings[0], DateTime.Parse(strings[1])));
            }
        }

        return snaps;
    }

    public void IncludeInSnapshot()
    {

    }
}