using Daemon.Backup.Services;
using Daemon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Backup.BackupTypes
{
    public class BackupService
    {
        protected FilesService files = new FilesService();



        public void SnapshotExists(string path, int idConfig)
        {
            // Check jestli snapshot
            // if false - vytvori ho

            if (File.Exists($@"{path}\.snapshot\snapshot.txt"))
                return;

            Directory.CreateDirectory($@"{path}\.snapshot\");
            File.Create($@"{path}\.snapshot\snapshot.txt").Dispose();
        }
        public int GetLastBackupNumber(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            List<DirectoryInfo> dirs = new List<DirectoryInfo>();
            try
            {
				dirs = dir.GetDirectories().ToList();
			}
            catch (Exception)
            {
                return 0;
                
            }

            
            //if (dirs.Count == 0)
            //    return 0;

            //string name = dirs.Last().FullName;

            //return int.Parse(name.Substring(name.LastIndexOf('_') + 1));

            return dirs.Count;
        }
    }
}
