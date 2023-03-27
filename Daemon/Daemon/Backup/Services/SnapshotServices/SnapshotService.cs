namespace Daemon.Backup.Services.SnapshotServices;

public class SnapshotService
{
    public List<SnappedFile> ReadSnappedFiles(string snapshotPath)
    {
        List<SnappedFile> snaps = new List<SnappedFile>();

        using (StreamReader sr = new StreamReader(snapshotPath))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] strings = line.Split('|');
                snaps.Add(new SnappedFile(strings[0], DateTime.Parse(strings[1]))); 
            }
            // while (sr.EndOfStream)
            // {
            //     string[] strings = sr.ReadLine()!.Split('|');
            //     snaps.Add(new SnappedFile(strings[0], DateTime.Parse(strings[1])));
            // }
        }

        return snaps;
    }

    public void IncludeInSnapshot()
    {

    }
}