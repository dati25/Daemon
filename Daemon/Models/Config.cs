using System.ComponentModel.DataAnnotations.Schema;

namespace Daemon.Models;
[Table("tbConfigs")]
public class Config
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string RepeatPeriod { get; set; } // Null = once
    public string? ExpirationDate { get; set; } // Null = never
    public bool? Compress { get; set; } // Default false
    public int? Retention { get; set; } // null = unlimitied
    public int? PackageSize { get; set; } // null = unlimited
    public bool? Status { get; set; }  //Default false (turned off) 
    [ForeignKey("IdConfig")] public List<Source>? Sources { get; set; }
    [ForeignKey("IdConfig")] public List<Destination>? Destinations { get; set; }
    [ForeignKey("IdConfig")] public List<Tasks>? Tasks { get; set; }
}