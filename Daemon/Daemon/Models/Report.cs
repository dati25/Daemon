﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon.Models
{
    public class Report
    {
        public int id { get; set; }

        public int idPC { get; set; }

        public bool Status { get; set; }

        public DateTime ReportTime { get; set; }

        public string Description { get; set; }
    }
}