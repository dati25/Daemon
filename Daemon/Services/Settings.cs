using Daemon.Models;
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
    public Report? SaveReport(Report report)
    {
        if (!Directory.Exists(SettingsConfig.SettingsDir))
            Directory.CreateDirectory(SettingsConfig.SettingsDir);

        if (File.Exists(SettingsConfig.ReportsPath))
            File.Delete(SettingsConfig.ReportsPath);

        using (var sw = new StreamWriter(SettingsConfig.ReportsPath, true))
        {
            if (new FileInfo("file").Length == 0)
                sw.Write(';');
            sw.WriteLine();
            sw.Write(JsonConvert.SerializeObject(report));
        }
        return report;
    }
    public List<Report>? ReadReports()
    {
        var reports = new List<Report>();

        using (var sr = new StreamReader(SettingsConfig.ReportsPath))
        {
            var splitReports = sr.ReadToEnd().Split(';').ToList();

            try
            {
                splitReports.ForEach(report => reports.Add(JsonConvert.DeserializeObject<Report>(report)!));
            }
            catch (Exception)
            {
                Console.WriteLine("Failed reports import");
                return null;
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
    private void UploadConfigs()
    {
        var client = new Client();
        var reports = this.ReadReports();
        
        if (reports == null) return;

        Parallel.ForEach(reports, report =>
        {
            if (client.PostReport(report).GetAwaiter().GetResult())
            {
                return;
            }
        });
    }
}