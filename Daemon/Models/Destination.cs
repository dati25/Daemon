using System.ComponentModel.DataAnnotations.Schema;

namespace Daemon.Models
{
    [Table("tbDestinations")]
    public class Destination
    {
        public int Id { get; set; }
        public int? IdConfig { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }

        public Destination(int? idConfig, string path, string type)
        {
            IdConfig = idConfig;
            Path = path;
            Type = type;
        }
    }
}
