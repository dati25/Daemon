using System.ComponentModel.DataAnnotations.Schema;

namespace Daemon.Models
{
    [Table("tbTasks")]
    public class Job
    {
        public int Id { get; set; }
        public int IdPc { get; set; }
        public int? IdConfig { get; set; }
    }
}
