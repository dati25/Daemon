using Daemon.Backup.Services;
using Daemon.Models;

namespace Daemon.Backup.BackupTypes;

public class BackupService
{
    protected FileService fs = new FileService();

    public void CreateSnapshot(string path)
    {
        DirectoryInfo di = Directory.CreateDirectory($@"{path}\.snapshot\");
        di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

        File.Create($@"{path}\.snapshot\snapshot.txt").Dispose();
    }

    public int GetBackupNumber(string path, Config config)
    {
        DirectoryInfo d = new DirectoryInfo(path);

        int fileCount = 1;
        int dirCount = 1;

        try
        {
            FileInfo[] items = d.GetFiles();
            int backups = items.Length + 1;

            Array.Sort(items, delegate (FileInfo f1, FileInfo f2)
            {
                return f1.LastWriteTime.CompareTo(f2.LastWriteTime);
            });

            string name = items[items.Length - 1].Name;
            string[] split = name.Split('_');
            string[] split2 = split[1].Split('.');
            int num = int.Parse(split2[0]);

            fileCount += num;
        }
        catch { }

        try
        {
            DirectoryInfo[] items = d.GetDirectories();
            int backups = items.Length + 1;

            Array.Sort(items, delegate (DirectoryInfo d1, DirectoryInfo d2)
            {
                return d1.LastWriteTime.CompareTo(d2.LastWriteTime);
            });

            string name = items[items.Length - 1].Name;
            string[] split = name.Split('_');
            int num = int.Parse(split[1]);

            dirCount += num;
        }
        catch { }

        if (fileCount > dirCount)
            return fileCount;
        else
            return dirCount;
    }

    public void DeleteBackup(string path)
    {
        if (File.Exists(path + ".zip"))
            File.Delete(path + ".zip");

        if (Directory.Exists(path))
            Directory.Delete(path, true);
    }
}