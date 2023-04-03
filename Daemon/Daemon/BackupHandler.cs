using Daemon.Models;

namespace Daemon;

public class BackupHandler
{
    private List<Config>? _configs;

    public BackupHandler(List<Config>? configs)
    {
        _configs = configs;
    }

    public void Begin()
    {
        if (_configs == null) return;

        foreach (var config in _configs)
            DistributeConfigs(config);
    }

    public void DistributeConfigs(Config config)
    {
        Backup b = new Backup(config);

        switch (config.Type.ToLower())
        {
            case "full":
                b.Execute();
                break;

            case "diff":
                b.Execute(true);
                break;

            case "incr":
                b.Execute(true, true);
                break;
        }
    }
}
