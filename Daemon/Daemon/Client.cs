using System.Net.Http.Json;
using System.Net.NetworkInformation;
using Daemon.Models;
using Newtonsoft.Json;

namespace Daemon
{
    public class Client
    {
        public HttpClient httpClient = new HttpClient() { BaseAddress = new Uri("http://localhost:5105") };

        public async Task<string?> GetPcId()
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




            HttpResponseMessage response = await httpClient.PostAsJsonAsync(httpClient.BaseAddress + "api/Computer", new Computer(physicalAddress.ToString(), ipv4Address!.ToString(), Environment.MachineName));

            return response.Content.ReadAsStringAsync().Result;
        }

		public async Task<int?> GetDeserializedPcId()
		{
            // {"idPC":60}
            // {"idPC":958191}
            // {"idPC":9}
            // {"idPC":818919}
            string? PcIdJson = await GetPcId();

            dynamic? obj = JsonConvert.DeserializeObject(PcIdJson);

            int? PcId = obj.idPc;

			return PcId;
		}
	}
}
