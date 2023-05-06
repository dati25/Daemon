using System.Net.NetworkInformation;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Daemon.Models;
using Daemon.Services;

namespace Daemon;
public class Client
{
    private readonly HttpClient client = new() { BaseAddress = new Uri("http://localhost:5105/") };
    private readonly SettingsConfig setttingsConfig = new();

    public async Task Register()
    {
        var s = new Settings();

        var pc = s.SavePc(await GetPc());
        var configs = s.SaveConfigs(await GetConfigs(pc));

        s.Update(pc, configs);
    }

    private async Task<Pc?> GetPc()
    {
        var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
        .FirstOrDefault(ni => ni.OperationalStatus == OperationalStatus.Up &&
                        ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                        ni.GetIPv4Statistics().UnicastPacketsReceived > 0);

        if (networkInterface == null)
            return null;

        var physicalAddress = networkInterface.GetPhysicalAddress();
        var ipProperties = networkInterface.GetIPProperties();
        var ipv4Address = ipProperties.UnicastAddresses
            .FirstOrDefault(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.Address;

        if (File.Exists(Path.Combine(setttingsConfig.SettingsDir, "pc.json"))) return null;
        if (!Directory.Exists(setttingsConfig.SettingsDir)) Directory.CreateDirectory(setttingsConfig.SettingsDir);

        try
        {
            var response = await client.PostAsJsonAsync(client.BaseAddress + "api/Computer", new Computer(physicalAddress.ToString(), ipv4Address!.ToString(), Environment.MachineName));
            var content = response.Content.ReadAsStringAsync().Result;
            var pc = new Pc { idPc = int.Parse(content) };

            return pc;
        }
        catch { return null; }
    }

    private async Task<List<Config>?> GetConfigs(Pc? pc)
    {
        if (pc == null) return null;

        var configIds = await GetConfigIds(pc);
        var configs = new List<Config>();

        if (configIds == null) return null;

        foreach (var id in configIds)
        {
            var response = await client.GetAsync(client.BaseAddress + "api/Config/" + id);
            var content = await response.Content.ReadAsStringAsync();
            var config = JsonConvert.DeserializeObject<Config>(content);

            if (config == null)
                return null;

            configs.Add(config);
        }

        return configs;
    }

    private async Task<List<int>?> GetConfigIds(Pc? pc)
    {
        if (pc == null) return null;

        try
        {
            var response = await client.GetAsync(client.BaseAddress + "api/tasks/" + pc.idPc);
            var content = response.Content.ReadAsStringAsync().Result;
            var configs = JsonConvert.DeserializeObject<List<int>>(content);

            return configs ?? null;
        }
        catch { return null; }
    }

    public async Task AddSnapshots()
    {
        var sc = new SettingsConfig();
        var files = new DirectoryInfo(sc.SnapshotsPath).GetFiles();

        foreach (var file in files)
        {
            var temp = file.Name.Split('_')[1];
            var configId = int.Parse(temp.Split('.')[0]);

            var c = GetConfigs(GetPc().Result).Result!.FirstOrDefault(c => c.Id == configId);
            Tasks? task = null;

            if (c != null)
                task = c.Tasks!.FirstOrDefault(t => t.IdPc == GetPc().Result!.idPc);

            if (c == null || task!.Snapshot != null) continue;
            var content = await File.ReadAllTextAsync(file.FullName);

            var idPc = GetPc().Result!.idPc;
            var ts = new Tasks() { IdPc = idPc, Snapshot = content };

            try
            {
                var response = await client.PutAsJsonAsync(client.BaseAddress + $"api/Config/{configId}", ts);
            }
            catch { }
        }
    }
}