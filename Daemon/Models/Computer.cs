using System.ComponentModel.DataAnnotations.Schema;

namespace Daemon.Models
{
    [Table("tbPC")]
    public class Computer
    {
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
        public string Name { get; set; }

        public Computer(string macAddress, string ipAddress, string name)
        {
            MacAddress = macAddress;
            IpAddress = ipAddress;
            Name = name;
        }
    }
}
