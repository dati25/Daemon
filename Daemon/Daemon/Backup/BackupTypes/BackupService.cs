using Daemon.Backup.Services;
using Daemon.Models;

namespace Daemon.Backup.BackupTypes;

public class BackupService
{
    protected FileService files = new FileService();

    public void CreateSnapshot(string path)
    {
        DirectoryInfo di = Directory.CreateDirectory($@"{path}\.snapshot\");
        di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

        File.Create($@"{path}\.snapshot\snapshot.txt").Dispose();
    }

    public int GetBackupNumber(string path, Config config)
    {
        DirectoryInfo d = new DirectoryInfo(path);


        try
        {
            FileInfo[] items = d.GetFiles();
            int backups = items.Length + 1;

            if (backups > config.Retention)
            {
                Array.Sort(items, delegate (FileInfo f1, FileInfo f2)
                {
                    return f1.LastWriteTime.CompareTo(f2.LastWriteTime);
                });

                items[0].Delete();
            }

            string name = items[items.Length - 1].Name;
            string[] split = name.Split('_');
            string[] split2 = split[1].Split('.');
            int num = int.Parse(split2[0]);

            return num + 1;
        }
        catch { }

        try
        {
            DirectoryInfo[] items = d.GetDirectories();
            int backups = items.Length + 1;

            if (backups > config.Retention)
            {
                Array.Sort(items, delegate (DirectoryInfo d1, DirectoryInfo d2)
                {
                    return d1.LastWriteTime.CompareTo(d2.LastWriteTime);
                });

                items[0].Delete(true);
            }

            string name = items[items.Length - 1].Name;
            string[] split = name.Split('_');
            int num = int.Parse(split[1]);

            return num + 1;
        }
        catch { }


        return 1;
    }
}