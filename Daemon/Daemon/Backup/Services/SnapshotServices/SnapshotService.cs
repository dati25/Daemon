using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Backup.Services.SnapshotServices
{
    public class SnapshotService
    {
        public List<SnappedFile> ReadSnappedFiles(string snapshotPath)
        {
            using (StreamReader sr = new StreamReader(snapshotPath))
            {





            }


        }


    }
}
