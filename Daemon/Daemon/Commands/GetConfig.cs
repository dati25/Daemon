using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Daemon.Models;

namespace Daemon.Commands
{
    public class GetConfig
    {
        Client client = new Client();
        public async Task<List<Config>> GetAsyncConfigs(int idPC)
        {
            string result = await client.httpClient.GetStringAsync($"/api/Config/");
            List<Config> configs = JsonConvert.DeserializeObject<List<Config>>(result);
            return configs;
        }
        public List<Config> GetDeserializedConfigs(int idPC)
        {
            return GetAsyncConfigs(idPC).GetAwaiter().GetResult();
        }
    }
}
