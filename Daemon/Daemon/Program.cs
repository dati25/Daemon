using Daemon.Backup.BackupTypes;
using Daemon.Backup;
using Daemon.Models;

namespace Daemon;

internal class Program
{
    public static async Task Main()
    {
        // Register r = new Register();
        // await r.RegisterPC();
        await Task.Delay(1);

        List<Source> sources = new List<Source>();
        sources.Add(new Source(157, @"C:\Users\shart\Desktop\Source1"));
        sources.Add(new Source(157, @"C:\Users\shart\Desktop\Source2"));
        sources.Add(new Source(157, @"C:\Users\shart\Desktop\Source3"));

        List<Destination> destinations = new List<Destination>();
        destinations.Add(new Destination(157, @"C:\Users\shart\Downloads\Dest1", "lcl"));
        destinations.Add(new Destination(157, @"C:\Users\shart\Downloads\Dest2", "lcl"));


        Config? config = new Config("diff", "* * * * *", DateTime.Now, false, 5, 3, 1, true, sources, destinations) { Id = 157 };

        DiffBackup backup = new DiffBackup(config);
        backup.Execute();
    }
}