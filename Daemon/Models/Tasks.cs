using System.ComponentModel.DataAnnotations.Schema;

namespace Daemon.Models;

[Table("tbTasks")]
public class Tasks
{
    public int IdPc { get; set; }
    public string? Snapshot { get; set; }

    public Tasks(int idPc, string? snapshot)
    {
        IdPc = idPc;
        Snapshot = snapshot;
    }
}