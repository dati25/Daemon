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
            Settings s = new Settings();

            Pc? pc = s.SavePc(await c.GetPc());
            List<Config>? configs = s.SaveConfig(await c.GetConfigs(pc));
        }
    }
}
