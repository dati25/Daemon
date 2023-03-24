using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Models
{
    public class UserPostRegister
    {
        public string MACAddress { get; set; }

        public string IPAdress { get; set; }

        public char Status { get; set; }

        public string Name { get; set; }
        public UserPostRegister(string IPAdress, string MacAddress)
        {
            this.IPAdress = IPAdress;
            this.MACAddress = MacAddress;
        }
    }
}
