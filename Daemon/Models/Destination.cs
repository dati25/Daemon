using System.ComponentModel.DataAnnotations.Schema;

namespace Daemon.Models;

[Table("tbDestinations")]
public class Destination
{
    public int Id { get; set; }
    public int? IdConfig { get; set; }
    public string? Path { get; set; }
    public string? Type { get; set; }
}