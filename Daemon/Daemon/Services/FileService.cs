using Daemon.Models;

namespace Daemon.Services
{
    public class FileService
    {
        public void Copy(string source, string dest, bool check = false, List<Snapshot>? snapshots = null)
        {
            if (!Directory.Exists(source)) return;

            var dir = new DirectoryInfo(Path.Combine(dest, new DirectoryInfo(source).Name));
            dir.Create();
            dest = Path.Combine(dest, dir.Name);

            foreach (var dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                if (check && snapshots != null)
                {
                    DirectoryInfo d = new DirectoryInfo(dirPath);
                    var snap = new Snapshot(d.FullName, d.LastWriteTime);

                    var snapshotMatch = snapshots.FirstOrDefault(s => s.Path == snap.Path);
                    if (snapshotMatch == null || snapshotMatch.LastModified.AddSeconds(1) < snap.LastModified)
                        Directory.CreateDirectory(dirPath.Replace(source, dest));
                }
                else
                    Directory.CreateDirectory(dirPath.Replace(source, dest));
            }

            foreach (var newPath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
            {
                if (check && snapshots != null)
                {
                    FileInfo d = new FileInfo(newPath);
                    var snap = new Snapshot(d.FullName, d.LastWriteTime);

                    var snapshotMatch = snapshots.FirstOrDefault(s => s.Path == snap.Path);
                    if (snapshotMatch == null || snapshotMatch.LastModified.AddSeconds(1) < snap.LastModified)
                        File.Copy(newPath, newPath.Replace(source, dest), true);
                }
                else
                    File.Copy(newPath, newPath.Replace(source, dest), true);
            }
        }
    }
}
