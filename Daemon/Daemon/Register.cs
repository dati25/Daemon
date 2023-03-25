using Daemon.Models;

namespace Daemon
{
    public class Register
    {
        Client c = new Client();


        public Register()
        {
            Client c = new Client();
        }

        public async Task RegisterPC()
        {
            Pc? pc = await c.GetPcId();
            List<Config>? configs = await c.GetConfigs(pc);

            Settings s = new Settings();
            s.SaveId(pc);
            s.SaveConfig(configs);
        }
    }
}
