using Daemon.Models;
using Newtonsoft.Json;

namespace Daemon.Services;
public class Settings
{
    private readonly SettingsConfig settingConfig = new();

    public Pc? SavePc(Pc? pc)
    {
        if (pc == null) return ReadPc();

        if (!Directory.Exists(this.settingConfig.SettingsDir))
            Directory.CreateDirectory(this.settingConfig.SettingsDir);

        if (File.Exists(this.settingConfig.PcPath))
            File.Delete(this.settingConfig.PcPath);

        File.WriteAllText(this.settingConfig.PcPath, JsonConvert.SerializeObject(pc, Formatting.Indented));

        return pc;
    }

    public List<Config>? SaveConfigs(List<Config>? configs)
    {
        if (configs == null) return ReadConfigs();

        if (!Directory.Exists(this.settingConfig.SettingsDir))
            Directory.CreateDirectory(this.settingConfig.SettingsDir);

        if (File.Exists(this.settingConfig.ConfigsPath))
            File.Delete(this.settingConfig.ConfigsPath);

        var jsons = new List<string>();

        configs.ForEach(c => jsons.Add(JsonConvert.SerializeObject(c, Formatting.Indented)));

        var json = string.Join(",\n", jsons);
        using var sw = new StreamWriter(this.settingConfig.ConfigsPath, true);
        sw.WriteLine("[");
        sw.WriteLine(json);
        sw.WriteLine("]");

        return configs;
    }

    public Pc? ReadPc()
    {
        if (!File.Exists(this.settingConfig.PcPath))
            return null;

        string json;
        using (var sr = new StreamReader(this.settingConfig.PcPath))
            json = sr.ReadToEnd();

        return JsonConvert.DeserializeObject<Pc>(json);
    }

    public List<Config>? ReadConfigs()
    {
        if (!File.Exists(this.settingConfig.ConfigsPath))
            return null;

        string json;
        using (var sr = new StreamReader(this.settingConfig.ConfigsPath))
            json = sr.ReadToEnd();

        return JsonConvert.DeserializeObject<List<Config>>(json);
    }

    public void Update(Pc? pc, List<Config>? configs)
    {
        if (pc == null || configs == null) return;

        File.WriteAllText(this.settingConfig.PcPath, string.Empty);
        File.WriteAllText(this.settingConfig.ConfigsPath, string.Empty);

        SavePc(pc);
        SaveConfigs(configs);
    }
}