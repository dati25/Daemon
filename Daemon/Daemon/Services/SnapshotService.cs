using Daemon.Client;
using Daemon.Models;

namespace Daemon.Services;
public class SnapshotService
{
    public void AddToSnapshot(Config config, string sourcePath)
    {
        SettingsConfig s = new();
        List<Snapshot> snaps = new();

        string snapshotPath = Path.Combine(s.SNAPSHOTSPATH, $"config_{config.Id}.txt");

        if (!Directory.Exists(s.SNAPSHOTSPATH))
            Directory.CreateDirectory(s.SNAPSHOTSPATH);

        if (!File.Exists(snapshotPath))
            File.Create(snapshotPath).Close();

        (sourcePath, snaps) = GetAllSnapshots(sourcePath, snaps);

        List<string> snapsText = new();
        snaps.ForEach(x => snapsText.Add(x.Path + "|" + x.LastModified.ToString()));

        using (StreamWriter sw = new StreamWriter(snapshotPath, true))
            snapsText.ForEach(x => sw.WriteLine(x));
    }

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


    private (string, List<Snapshot>) GetAllSnapshots(string sourcePath, List<Snapshot> snaps)
    {
        foreach (string filePath in Directory.GetFileSystemEntries(sourcePath))
        {
            FileSystemInfo fileSystemInfo = new FileInfo(filePath);
            snaps.Add(new Snapshot(fileSystemInfo.FullName, fileSystemInfo.LastWriteTime));
        }

        foreach (string subDir in Directory.GetDirectories(sourcePath))
        {
            GetAllSnapshots(subDir, snaps);
        }

        return (sourcePath, snaps);
    }
}