using Daemon.Services;

namespace Daemon;
internal abstract class Program
{
    public static async Task Main()
    {
        // var c = new Client();
        var s = new Settings();

        // await c.Register();

        // BackupHandler bh = new(s.ReadConfigs());
        // bh.Begin();

        await Task.Delay(1);

        var configs = s.ReadConfigs();

        BackupHandler bh = new(configs);
        bh.Begin();
    }
}