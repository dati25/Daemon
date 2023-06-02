using Daemon.Models;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

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
    public Report? SaveReport(Report report)
    {
        if (!Directory.Exists(SettingsConfig.SettingsDir))
            Directory.CreateDirectory(SettingsConfig.SettingsDir);

        using (var sw = new StreamWriter(SettingsConfig.ReportsPath, true))
        {
            string line = JsonConvert.SerializeObject(report);
            sw.WriteLine(line);
        }
        return report;
    }
    public List<Report>? ReadReports()
    {
        var reports = new List<Report>();

        using (var sr = new StreamReader(SettingsConfig.ReportsPath))
        {
            while (!sr.EndOfStream)
            {
                var line= sr.ReadLine();
                var report = JsonConvert.DeserializeObject<Report>(line!);   
                reports.Add(report!);   
            }
            return reports;
        }
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
        using (var sw = new StreamWriter(SettingsConfig.ConfigsPath, true))
        {
            sw.WriteLine("[");
            sw.WriteLine(json);
            sw.WriteLine("]");
        }
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
    public async void Update(ScheduleService schedule)
    {
        Client client = new Client();

        if (this.ReadPc() == null)
        {
            if (await client.Register() == null)
                return;
        }

        List<Config>? configs = client.GetConfigs(this.ReadPc()).GetAwaiter().GetResult()!;

        if (configs == null)
            return;

        configs.ForEach(config =>
        {
            schedule.UpdateConfigTrigger(config).GetAwaiter();
        });

        schedule.DeleteUnassignedConfigs();
        this.SaveConfigs(configs!);

        var pc = this.ReadPc();
        if (pc == null) return;
        var newStatus = client.GetPcStatus(pc!).GetAwaiter().GetResult();
        if (pc!.Status != newStatus && pc!.Status != null)
        {
            pc.Status = newStatus;
            this.SavePc(pc);
        }
    }
    public bool UploadReports()
    {
        var client = new Client();
        var reports = this.ReadReports();
        
        if (reports!.Count == 0) return false;

        foreach (var report in reports)
        {
            if (!client.PostReport(report).GetAwaiter().GetResult())
            {
                return false;
            }
        }
        File.WriteAllText(SettingsConfig.ReportsPath, String.Empty);
        return true;
    }
    private Report GetReport(Config config, char status, string? description = null)
    {
        Settings settings = new();
        var pc = settings.ReadPc();

        return new Report(pc!.idPc, config.Id, status, DateTime.Now, description);
    }

}