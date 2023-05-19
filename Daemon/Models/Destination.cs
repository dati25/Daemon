using Newtonsoft.Json;
using Quartz.Impl.Triggers;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Daemon.Models;

[Table("tbDestinations")]
public class Destination
{
    public int Id { get; set; }
    public string Path { get; set; }
    public bool Type { get; set; }
}