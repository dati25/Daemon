using Daemon.Models;

namespace Daemon.Services;
public class SnapshotService
{
    public void AddToSnapshot(string sourcePath, string snapshotPath)
    {
        List<Snapshot> snaps = new();

        snaps = GetAllSnapshots(sourcePath, snaps);

        List<string> snapsText = new();
        snaps.ForEach(x => snapsText.Add(x.Path + "|" + x.LastModified));

        using var sw = new StreamWriter(snapshotPath, true);
        snapsText.ForEach(x => sw.WriteLine(x));
    }

    public List<Snapshot> ReadSnapshots(string snapshotPath)
    {
        var snaps = new List<Snapshot>();

        using var sr = new StreamReader(snapshotPath);
        while (sr.ReadLine() is { } line)
        {
            var strings = line.Split('|');
            snaps.Add(new Snapshot(strings[0], DateTime.Parse(strings[1])));
        }

        return snaps;
    }
    public string ReadSnapshot(string snapshotPath)
    {
        using(var sr = new StreamReader(snapshotPath))
        {
            return sr.ReadToEnd();
        }
    }


    private List<Snapshot> GetAllSnapshots(string sourcePath, List<Snapshot> snaps)
    {
        var paths = Directory.GetFileSystemEntries(sourcePath);
        snaps.AddRange(paths.Select(filePath => new FileInfo(filePath)).Select(fileSystemInfo => new Snapshot(fileSystemInfo.FullName, fileSystemInfo.LastWriteTime)));

        foreach (var subDir in Directory.GetDirectories(sourcePath))
            GetAllSnapshots(subDir, snaps);

        return snaps;
    }
}