using System.ComponentModel.DataAnnotations.Schema;

namespace Daemon.Models;

[Table("tbTasks")]
public class Tasks
{
    public int IdPc { get; set; }
    public int IdConfig { get; set; }
    public string? Snapshot { get; set; }

    public Tasks(int idPc, int idConfig, string? snapshot)
    {
        IdPc = idPc;
        IdConfig = idConfig;
        Snapshot = snapshot;
    }
}