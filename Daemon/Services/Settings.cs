﻿using Daemon.Models;
using Newtonsoft.Json;

namespace Daemon.Services;
public class Settings
{

    public Pc? SavePc(Pc? pc)
    {
        if (pc == null) return ReadPc();

        if (!Directory.Exists(SettingsConfig.SettingsDir))
            Directory.CreateDirectory(SettingsConfig.SettingsDir);

        if (File.Exists(SettingsConfig.PcPath))
            File.Delete(SettingsConfig.PcPath);

        File.WriteAllText(SettingsConfig.PcPath, JsonConvert.SerializeObject(pc, Formatting.Indented));

        return pc;
    }

    public List<Config>? SaveConfigs(List<Config>? configs)
    {
        if (configs == null) return ReadConfigs();

        if (!Directory.Exists(SettingsConfig.SettingsDir))
            Directory.CreateDirectory(SettingsConfig.SettingsDir);

        if (File.Exists(SettingsConfig.ConfigsPath))
            File.Delete(SettingsConfig.ConfigsPath);

        var jsons = new List<string>();

        configs.ForEach(c => jsons.Add(JsonConvert.SerializeObject(c, Formatting.Indented)));

        var json = string.Join(",\n", jsons);
        using var sw = new StreamWriter(SettingsConfig.ConfigsPath, true);
        sw.WriteLine("[");
        sw.WriteLine(json);
        sw.WriteLine("]");

        return configs;
    }

    public Pc? ReadPc()
    {
        if (!File.Exists(SettingsConfig.PcPath))
            return null;

        string json;
        using (var sr = new StreamReader(SettingsConfig.PcPath))
            json = sr.ReadToEnd();

        return JsonConvert.DeserializeObject<Pc>(json);
    }

    public List<Config>? ReadConfigs()
    {
        if (!File.Exists(SettingsConfig.ConfigsPath))
            return null;

        string json;
        using (var sr = new StreamReader(SettingsConfig.ConfigsPath))
            json = sr.ReadToEnd();

        return JsonConvert.DeserializeObject<List<Config>>(json);
    }

    public void Save(Pc? pc, List<Config>? configs)
    {
        if (pc == null || configs == null) return;

        File.WriteAllText(SettingsConfig.PcPath, string.Empty);
        File.WriteAllText(SettingsConfig.ConfigsPath, string.Empty);

        SavePc(pc);
        SaveConfigs(configs);
    }
    public void Update(ScheduleService schedule)
    {
        Client client = new Client();

        List<Config>? configs = client.GetConfigs(this.ReadPc()).GetAwaiter().GetResult()!;

        if (configs == null)
            return;

        configs.ForEach(config =>
        {
            schedule.UpdateConfigTrigger(config);
        });

        schedule.DeleteUnassignedConfigs(configs);

        this.SaveConfigs(configs!);
    }
}