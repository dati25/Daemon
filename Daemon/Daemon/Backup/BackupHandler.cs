using Daemon.Backup.BackupTypes;
using Daemon.Models;

namespace Daemon.Backup;

public class BackupHandler
{
    private Client client = new Client();
    private List<Config>? configs;

    public BackupHandler()
    {
        configs = client.GetConfigs(client.GetPc().GetAwaiter().GetResult()).GetAwaiter().GetResult();
    }

    public void Begin()
    {
        if (configs == null) return;

        foreach (var config in configs)
            this.DistributeConfigs(config);
    }

    public void DistributeConfigs(Config config)
    {
        switch (config.Type.ToLower())
        {
            case "full":
                FullBackup full = new FullBackup(config);
                full.Execute();
                break;

            case "diff":
                break;

            case "incr":
                break;
        }
    }
}
