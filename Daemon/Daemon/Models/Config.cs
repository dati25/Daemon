using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Models
{
    public class Config
    {
        public int id { get; set; }
        public string Type { get; set; }
        public string? RepeatPeriod { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool? Compress { get; set; }
        public int? Retention { get; set; }
        public int? PackageSize { get; set; }
        public int CreatedBy { get; set; }
        public bool? Status { get; set; }
        public List<Source>? Sources { get; set; } = new List<Source>();
        public List<Destination>? Destinations { get; set; } = new List<Destination>();
        public List<Job>? Jobs { get; set; } = new List<Job>();



    }
}
