using System.Net.NetworkInformation;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Daemon.Models;
using Daemon.Services;
using System.Text.Json;

namespace Daemon;
public class Client
{
    private readonly HttpClient client = new() { BaseAddress = new Uri("http://localhost:5105/") };
    
    public async Task Register()
    {
        var s = new Settings();

        var pc = s.SavePc(await GetPc());
        var configs = s.SaveConfigs(await GetConfigs(pc));

        s.Save(pc, configs);
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

        if (File.Exists(Path.Combine(SettingsConfig.SettingsDir, "pc.json"))) return null;
        if (!Directory.Exists(SettingsConfig.SettingsDir)) Directory.CreateDirectory(SettingsConfig.SettingsDir);

        try
        {
            var response = await client.PostAsJsonAsync(client.BaseAddress + "api/Computer", new Computer(physicalAddress.ToString(), ipv4Address!.ToString(), Environment.MachineName));
            var content = response.Content.ReadAsStringAsync().Result;
            var pc = new Pc { idPc = int.Parse(content) };

            return pc;
        }
        catch { return null; }
    }

    public async Task<List<Config>?> GetConfigs(Pc? pc)
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

    public async Task AddSnapshot(int configId)
    {
        var settings = new Settings();
        var snapshotService = new SnapshotService();

        int idPc = settings.ReadPc()!.idPc;
        string snapshot = snapshotService.ReadSnapshot(Path.Combine(SettingsConfig.SnapshotsPath, $"config_{configId}.txt"));


        var response = await this.client.PutAsJsonAsync(client.BaseAddress + $"api/Snapshot/{idPc}/{configId}", snapshot);
    }
    public async Task<Snapshot?> GetSnapshot(Config config)
    {
        Settings settings = new();
        var pc = settings.ReadPc();

        if (pc == null)
            return null;

        var response = await this.client.GetAsync(this.client.BaseAddress + $"api/Snapshot/{pc.idPc}");
        if (!response.IsSuccessStatusCode)
            return null;

        var content = response.Content.ReadAsStringAsync().Result;
        var snapshots = System.Text.Json.JsonSerializer.Deserialize<List<Snapshot>>(content);
        if (snapshots == null)
            return null;

        return snapshots!.Where(snap => snap.ConfigId == config.Id).FirstOrDefault();
    }
}