using Daemon.Backup.BackupTypes;
using Daemon.Models;

namespace Daemon;

internal class Program
{
    public static async Task Main()
    {
        // Client c = new Client();
        // await c.Register();


        await Task.Delay(1);

        List<Source> sources = new List<Source>();
        sources.Add(new Source(157, @"C:\Users\shart\Desktop\Source1"));
        sources.Add(new Source(157, @"C:\Users\shart\Desktop\Source2"));
        sources.Add(new Source(157, @"C:\Users\shart\Desktop\Source3"));

        List<Destination> destinations = new List<Destination>();
        destinations.Add(new Destination(157, @"C:\Users\shart\Downloads\Dest1", "lcl"));
        destinations.Add(new Destination(157, @"C:\Users\shart\Downloads\Dest2", "lcl"));

        List<Tasks> tasks = new List<Tasks>();


        Config? config = new Config("full", "* * * * *", DateTime.Now.ToString(), true, 5, 3, 1, true, sources, destinations, tasks) { Id = 157 };

        FullBackup backup = new FullBackup(config);
        backup.Execute();
    }
}