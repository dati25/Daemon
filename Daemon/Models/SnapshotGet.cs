using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Daemon.Models
{
    internal class SnapshotGet
    {
        public int ComputerId { get; set; }
        public int IdConfig { get; set; }
        public string? Snapshot { get; set; } = null;


    }
    internal class NewSnapShot
    {
        public partial class Snapshot
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("snapshot")]
            public string SnapshotSnapshot { get; set; }
        }
    }
}
