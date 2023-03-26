using Daemon.Backup.BackupTypes;
using Daemon.Models;
using Newtonsoft.Json;

namespace Daemon
{
    internal class Program
    {
        public static async Task Main()
        {
            //Register r = new Register();
            //await r.RegisterPC();

            //Application app = new Application();
            //app.Execute();

            Source source1 = new Source(5, @"C:\FOO");
            Destination destination1 = new Destination(5, @"C:\BACK", "lcl");
            List<Source> sources = new List<Source>();
            List<Destination> destinations = new List<Destination>();
            sources.Add(source1);
            destinations.Add(destination1);
            List<Job> jobs = new List<Job>();

            Config config = new Config("Full", "* * * * *",DateTime.Now,true,5,1,1,true,sources,destinations,jobs);

            FullBackup backup = new FullBackup(config);
            backup.Execute();
		}
    }
}