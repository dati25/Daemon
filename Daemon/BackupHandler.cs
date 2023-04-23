using Daemon.Models;

namespace Daemon;
public class BackupHandler
{
    private readonly List<Config>? _configs;

    public BackupHandler(List<Config>? configs)
    {
        _configs = configs;
    }

    public void Begin()
    {
        if (_configs == null) return;

        foreach (var config in _configs)
            ExecuteConfigs(config);
    }

    private void ExecuteConfigs(Config config)
    {
        var b = new Backup(config);

        switch (config.Type!.ToLower())
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
