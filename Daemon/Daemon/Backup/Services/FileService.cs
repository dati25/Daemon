using Daemon.Backup.Services.SnapshotServices;

namespace Daemon.Backup.Services
{
    public class FileService
    {
        public void Copy(string source, string dest)
        {
            if (!Directory.Exists(source)) return;

            var dir = new DirectoryInfo(Path.Combine(dest, new DirectoryInfo(source).Name));
            dir.Create();
            dest = Path.Combine(dest, dir.Name);

            foreach (var dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(source, dest));

            foreach (var newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(source, dest), true);
        }

        public void CopyWithSnapshot(string source, string dest)
        {
            StreamWriter sw = new StreamWriter(dest + @"\.snapshot\snapshot.txt", true);

            if (!Directory.Exists(source)) return;

            var dir = new DirectoryInfo(Path.Combine(dest, new DirectoryInfo(source).Name));
            dir.Create();
            dest = Path.Combine(dest, dir.Name);

            foreach (var dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(source, dest));
                DirectoryInfo d = new DirectoryInfo(dirPath);

                sw.WriteLine(dirPath + "|" + d.LastWriteTime);
            }

            foreach (var newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(source, dest), true);
                FileInfo f = new FileInfo(newPath);

                sw.WriteLine(newPath + "|" + f.LastWriteTime);
            }

            sw.Close();
        }

        public void CopyWithSnapshotCheck(string source, string dest, List<SnappedFile> snapshot)
        {
            if (!Directory.Exists(source)) return;

            var dir = new DirectoryInfo(Path.Combine(dest, new DirectoryInfo(source).Name));
            dir.Create();
            dest = Path.Combine(dest, dir.Name);

            foreach (var dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                DirectoryInfo d = new DirectoryInfo(dirPath);

                if (snapshot.Any(item => item.FullPath == dirPath/* && item.LastDateModified == d.LastWriteTime*/))
                    continue;

                Directory.CreateDirectory(dirPath.Replace(dirPath, Path.Combine(dest, dirPath.Substring(dirPath.LastIndexOf('\\') + 1))));
            }

            foreach (var newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                FileInfo f = new FileInfo(newPath);

                if (snapshot.Any(item => item.FullPath == newPath/* && item.LastDateModified == f.LastWriteTime*/))
                    continue;

                File.Copy(newPath, newPath.Replace(source, dest), true);
            }
        }
    }
}
