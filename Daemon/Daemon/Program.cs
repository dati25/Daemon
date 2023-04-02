using Daemon.Models;

namespace Daemon;

internal class Program
{
    public static async Task Main()
    {
        // Client c = new Client();
        // await c.Register();


        await Task.Delay(1);

        List<Source> sources = new()
        {
            new Source(157, @"C:\Users\shart\Desktop\Source1"),
            new Source(157, @"C:\Users\shart\Desktop\Source2"),
            new Source(157, @"C:\Users\shart\Desktop\Source3")
        };

        List<Destination> destinations = new()
        {
            new Destination(157, @"C:\Users\shart\Downloads\Dest1", "lcl"),
            new Destination(157, @"C:\Users\shart\Downloads\Dest2", "lcl")
        };

        List<Tasks> tasks = new();


        Config? config = new("full", "* * * * *", DateTime.Now.ToString(), false, 5, 3, 1, true, sources, destinations, tasks) { Id = 157 };

        Backup b = new(config);
        b.Execute(true, true);
    }
}