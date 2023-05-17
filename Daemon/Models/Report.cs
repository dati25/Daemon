using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int IdConfig { get; set; }
        public int IdPc { get; set; }
        public bool Status { get; set; }
        public DateTime ReportTime { get; set; }
        public string? Description { get; set; }

        public Report(int idPc, int idConfig, bool status, DateTime reportTime, string? description)
        {
            this.IdPc = idPc;
            this.IdConfig = idConfig;
            this.Status = status;
            this.ReportTime = reportTime;
            this.Description = description;
        }


    }
}
