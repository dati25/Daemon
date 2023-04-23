using System.ComponentModel.DataAnnotations.Schema;

namespace Daemon.Models;
[Table("tbConfigs")]
public class Config
{
    public int? Id { get; set; }
    public string? Type { get; set; }
    public string? RepeatPeriod { get; set; }
    public string? ExpirationDate { get; set; }
    public bool? Compress { get; set; }
    public int? Retention { get; set; }
    public int? PackageSize { get; set; }
    public bool? Status { get; set; }
    [ForeignKey("IdConfig")] public List<Source>? Sources { get; set; }
    [ForeignKey("IdConfig")] public List<Destination>? Destinations { get; set; }
    [ForeignKey("IdConfig")] public List<Tasks>? Tasks { get; set; }
}