using System.Net.NetworkInformation;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Daemon.Models;
using Daemon.Services;

namespace Daemon;
public class Client
{
    HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:5105/") };

    public async Task Register()
    {
        Settings s = new Settings();

        Pc? pc = s.SavePc(await GetPc());
        List<Config>? configs = s.SaveConfigs(await GetConfigs(pc));

        s.Update(pc, configs);
    }

    public async Task<Pc?> GetPc()
    {
        var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
        .FirstOrDefault(ni => ni.OperationalStatus == OperationalStatus.Up &&
                        ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                        ni.GetIPv4Statistics().UnicastPacketsReceived > 0);

        if (networkInterface == null)
            return null;

        PhysicalAddress? physicalAddress;
        System.Net.IPAddress? ipv4Address;

        physicalAddress = networkInterface.GetPhysicalAddress();
        var ipProperties = networkInterface.GetIPProperties();
        ipv4Address = ipProperties.UnicastAddresses
            .FirstOrDefault(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.Address;

        if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup", "pc.json")))
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(client.BaseAddress + "api/Computer", new Computer(physicalAddress.ToString(), ipv4Address!.ToString(), Environment.MachineName));

                string content = response.Content.ReadAsStringAsync().Result;

                Pc? pc = JsonConvert.DeserializeObject<Pc>(content);

                if (pc == null)
                    return null;

                return pc;
            }
            catch { return null; }

        return null;
    }

    public async Task<List<Config>?> GetConfigs(Pc? pc)
    {
        if (pc == null) return null;

        List<int>? configIds = await GetConfigIds(pc);
        List<Config>? configs = new List<Config>();

        if (configIds == null)
            return null;

        foreach (int id in configIds)
        {
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress + "api/Config/" + id);

            string content = await response.Content.ReadAsStringAsync();

            Config? config = JsonConvert.DeserializeObject<Config>(content);

            if (config == null)
                return null;

            configs.Add(config);
        }

        return configs;
    }

    public async Task<List<int>?> GetConfigIds(Pc? pc)
    {
        if (pc == null) return null;

        try
        {
            HttpResponseMessage response = await client.GetAsync(client.BaseAddress + "api/tasks/" + pc.idPc);

            string content = response.Content.ReadAsStringAsync().Result;

            List<int>? configs = JsonConvert.DeserializeObject<List<int>>(content);

            if (configs == null)
                return null;

            return configs;
        }
        catch { return null; }
    }

    public async Task AddSnapshots()
    {
        SettingsConfig sc = new SettingsConfig();
        FileInfo[] files = new DirectoryInfo(sc.SNAPSHOTSPATH).GetFiles();

        foreach (FileInfo file in files)
        {
            string temp = file.Name.Split('_')[1];
            int configId = int.Parse(temp.Split('.')[0]);

            Config? c = GetConfigs(GetPc().Result).Result!.Where(c => c.Id == configId).FirstOrDefault();
            Tasks? t = null;

            if (c != null)
                t = c.Tasks!.Where(t => t.IdPc == GetPc().Result!.idPc).FirstOrDefault();

            if (c != null && t!.Snapshot == null)
            {
                string content = File.ReadAllText(file.FullName);

                int idPc = GetPc().Result!.idPc;
                Tasks ts = new Tasks(idPc, content);

                try
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync(client.BaseAddress + $"api/Config/{configId}", ts);
                }
                catch { }
            }
        }
    }
}