using Daemon.Models;
using Newtonsoft.Json;

namespace Daemon
{
    public class Settings
    {
        public void SaveId(Pc? pc)
        {
            if (pc == null) return;

            string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup");
            string settingsPath = Path.Combine(dataDir, "settings.json");

            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            using (StreamWriter sw = new StreamWriter(settingsPath, true))
            {
                // sw.WriteLine("[");
                sw.WriteLine(JsonConvert.SerializeObject(pc));
            }
        }

        public void SaveConfig(List<Config>? configs)
        {
            if (configs == null) return;

            string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup");
            string settingsPath = Path.Combine(dataDir, "settings.json");

            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            using (StreamWriter sw = new StreamWriter(settingsPath, true))
            {
                for (int i = 0; i < configs.Count; i++)
                {
                    sw.WriteLine(JsonConvert.SerializeObject(configs[i]));
                    sw.WriteLine(JsonConvert.SerializeObject(configs[i].Sources));
                    sw.WriteLine(JsonConvert.SerializeObject(configs[i].Destinations));

                    // if (i == configs.Count - 1)
                    //     sw.WriteLine(JsonConvert.SerializeObject(configs[i].Destinations));
                    // else
                    //     sw.WriteLine(JsonConvert.SerializeObject(configs[i].Destinations) + ",");
                }

                // sw.WriteLine("]");
            }
        }
    }
}
