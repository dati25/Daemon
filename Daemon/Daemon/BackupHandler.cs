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
        switch (config.Type.ToLower())
        {
            case "full":

                break;

            case "diff":
                break;

            case "incr":
                break;
        }
    }
}
