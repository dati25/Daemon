namespace Daemon;
internal class Program
{
    public static async Task Main()
    {
        Client c = new Client();
        await c.Register();

        BackupHandler bh = new BackupHandler(c.GetConfigs(c.GetPc().Result).Result);
        bh.Begin();
    }
}