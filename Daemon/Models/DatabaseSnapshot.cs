using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Models
{
    internal class DatabaseSnapshot
    {
        public int IdConfig { get; set; }
        public int ComputerId { get; set; }
        public string Snapshot { get; set; }
    }
}
