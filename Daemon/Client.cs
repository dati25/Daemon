using System.Net.NetworkInformation;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Daemon.Models;
using Daemon.Services;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Daemon;
public class Client
{
    private readonly HttpClient client = new() { BaseAddress = new Uri("http://localhost:5105/") };
    private Settings settings = new();
    public async Task Register()
    {
        var s = new Settings();

        this.pc = s.SavePc(await GetPc());
        var configs = s.SaveConfigs(await GetConfigs(pc));

        s.Save(pc, configs);
    }
    private async Task<Pc?> GetPc()
    {
        if (File.Exists(Path.Combine(SettingsConfig.SettingsDir, "pc.json"))) return null;

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

        if (!Directory.Exists(SettingsConfig.SettingsDir)) Directory.CreateDirectory(SettingsConfig.SettingsDir);

        try
        {
            var response = await client.PostAsJsonAsync(client.BaseAddress + "api/Computer", new Computer(physicalAddress.ToString(), ipv4Address!.ToString(), Environment.MachineName));
            var content = response.Content.ReadAsStringAsync().Result;
            var pc = new Pc { idPc = int.Parse(content) };
            pc.Status = await this.GetPcStatus(pc);
            return pc;
        }
        catch { return null; }
    }
    public async Task<char?> GetPcStatus(Pc pc)
    {
         var response = await this.client.GetAsync(client.BaseAddress + $"api/Sessions/{pc.idPc}");
        var content = response.Content.ReadAsStringAsync().Result;
            var stringStatus = JsonConvert.DeserializeObject<string>(content);
        if (stringStatus == null)
            return null;
        return Convert.ToChar(stringStatus);
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
        var snapshotService = new SnapshotService();

        int idPc = this.settings.ReadPc()!.idPc;
        string snapshot = snapshotService.ReadSnapshot(Path.Combine(SettingsConfig.SnapshotsPath, $"config_{configId}.txt"));

        //    var c = GetConfigs(GetPc().Result).Result!.FirstOrDefault(c => c.Id == configId);

        var response = await this.client.PutAsJsonAsync(client.BaseAddress + $"api/Snapshot/{idPc}/{configId}", snapshot);
    }
    public async Task<Snapshot?> GetSnapshot(Config config)
    {
        var pc = this.settings.ReadPc();

        if (pc == null)
            return null;

        var response = await this.client.GetAsync(this.client.BaseAddress + $"api/Snapshot/{pc.idPc}");
        if (!response.IsSuccessStatusCode)
            return null;

        //    try
        //    {
        //        var response = await client.PutAsJsonAsync(client.BaseAddress + $"api/Config/{configId}", ts);
        //    }
        //    catch { }
        //}

        await this.client.PutAsJsonAsync(client.BaseAddress + "api/Snapthots")
    }
    public async Task<bool> PostReport(Config config, bool status, string? description = null)
    {
        var pc = this.settings.ReadPc();

        try
        {
            var response = await client.PostAsJsonAsync(client.BaseAddress + "api/Report", new Report(pc!.idPc, config.Id, status, DateTime.Now, description));
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
public void ConnectToFTP(Source source)
{

}
}