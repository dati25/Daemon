using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Models
{
    public class Destination
    {
        public int id { get; set; }
        public bool Type { get; set; } //false = fileSystem ; true = FTP
        public string Path { get; set; }
    }
}
