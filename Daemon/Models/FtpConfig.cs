using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daemon.Models
{
    public class _FtpConfig
    {
        //ftp://<user>:<password>@<host>:<port>//<folder>
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string FilePath { get; set; }

        public _FtpConfig(string ftpConfig)
       {
            var match = Regex.Match(ftpConfig, @"^ftp://(?<user>[a-zA-Z\.\-_]+):(?<password>.[^@]+)\@(?<host>[0-9\.]+):(?<port>\d+)//(?<filePath>.*)");
            this.UserName = match.Groups["user"].Value;
            this.Password = match.Groups["password"].Value;
            this.Host = match.Groups["host"].Value;
            this.Port = match.Groups["port"].Value;
            this.FilePath = match.Groups["filePath"].Value;
        }


    }
}
