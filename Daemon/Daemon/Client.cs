using System.Net.Http.Json;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using Daemon.Models;

namespace Daemon
{
    public class Client
    {
        HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:5105/") };

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

                string content = response.Content.ReadAsStringAsync().Result;

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
    }
}
