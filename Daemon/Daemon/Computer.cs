using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon
{
    public class Computer
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
        public bool Status { get; set; }
    }
}
