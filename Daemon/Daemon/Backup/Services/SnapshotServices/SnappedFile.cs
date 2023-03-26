using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Backup.Services.SnapshotServices
{
    public class SnappedFile
    {
        public string FullPath { get; set; }
        public DateTime LastDateModified { get; set; }

        public SnappedFile(string fullPath, DateTime lastDateModified)
        {
            FullPath = fullPath;
            LastDateModified = lastDateModified;
        }
    }
}
