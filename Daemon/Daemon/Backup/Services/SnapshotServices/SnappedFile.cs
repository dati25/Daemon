using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Backup.Services.SnapshotServices
{
    public class SnappedFile
    {
        public string RelativePath { get; set; }
        public string SourcePath { get; set; }
        public DateTime LastDateModified { get; set; }


        public SnappedFile(string relativePath, string sourcePath, DateTime lastDateModified)
        {
            RelativePath = relativePath;
            SourcePath = sourcePath;
            LastDateModified = lastDateModified;
        }
    }
}
