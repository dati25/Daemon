using Daemon.Models;
using Newtonsoft.Json;

namespace Daemon.Services;
public class Settings
{
    private readonly SettingsConfig _sc = new();

    public Pc? SavePc(Pc? pc)
    {
        if (pc == null) return ReadPc();

        if (!Directory.Exists(_sc.SettingsDir))
            Directory.CreateDirectory(_sc.SettingsDir);

        if (File.Exists(_sc.PcPath))
            File.Delete(_sc.PcPath);

        File.WriteAllText(_sc.PcPath, JsonConvert.SerializeObject(pc, Formatting.Indented));

        return pc;
    }

    public List<Config>? SaveConfigs(List<Config>? configs)
    {
        if (configs == null) return ReadConfigs();

        if (!Directory.Exists(_sc.SettingsDir))
            Directory.CreateDirectory(_sc.SettingsDir);

        if (File.Exists(_sc.ConfigsPath))
            File.Delete(_sc.ConfigsPath);

        var jsons = new List<string>();

        configs.ForEach(c => jsons.Add(JsonConvert.SerializeObject(c, Formatting.Indented)));

        var json = string.Join(",\n", jsons);
        using var sw = new StreamWriter(_sc.ConfigsPath, true);
        sw.WriteLine("[");
        sw.WriteLine(json);
        sw.WriteLine("]");

        return configs;
    }

    public Pc? ReadPc()
    {
        if (!File.Exists(_sc.PcPath))
            return null;

        string json;
        using (var sr = new StreamReader(_sc.PcPath))
            json = sr.ReadToEnd();

        return JsonConvert.DeserializeObject<Pc>(json);
    }

    public List<Config>? ReadConfigs()
    {
        if (!File.Exists(_sc.ConfigsPath))
            return null;

        string json;
        using (var sr = new StreamReader(_sc.ConfigsPath))
            json = sr.ReadToEnd();

        return JsonConvert.DeserializeObject<List<Config>>(json);
    }

    public void Update(Pc? pc, List<Config>? configs)
    {
        if (pc == null || configs == null) return;

        File.WriteAllText(_sc.PcPath, string.Empty);
        File.WriteAllText(_sc.ConfigsPath, string.Empty);

        SavePc(pc);
        SaveConfigs(configs);
    }
}