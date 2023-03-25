using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Models
{
    
    public class Computer
    {
		public string MacAddress { get; set; }
		public string IpAddress { get; set; }
        public string Name { get; set; }

		public Computer(string macAddress, string ipAddress, string name)
        {
            this.MacAddress = macAddress;
            this.IpAddress = ipAddress;
            this.Name = name;
        }
    }
}
