using Daemon.Models;
using Newtonsoft.Json;

namespace Daemon
{
    public class Settings
    {
        public Pc? SavePc(Pc? pc)
        {
            if (pc == null) return ReadPc();

            string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup");
            string settingsPath = Path.Combine(dataDir, "settings.json");

            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            if (File.Exists(settingsPath))
                File.Delete(settingsPath);

            using (StreamWriter sw = new StreamWriter(settingsPath, true))
            {
                sw.WriteLine("[");
                sw.WriteLine(JsonConvert.SerializeObject(pc));
                sw.WriteLine(",");
            }

            return pc;
        }

        public List<Config>? SaveConfig(List<Config>? configs)
        {
            if (configs == null) return ReadConfigs();

            string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup");
            string settingsPath = Path.Combine(dataDir, "settings.json");

            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            List<string> jsons = new List<string>();

            configs.ForEach(c => jsons.Add(JsonConvert.SerializeObject(c, Formatting.Indented)));

            string json = string.Join(",\n", jsons);
            using (StreamWriter sw = new StreamWriter(settingsPath, true))
            {
                sw.WriteLine("[");
                sw.WriteLine(json);
                sw.WriteLine("]");
                sw.WriteLine("]");
            }

            return configs;
        }

        public Pc? ReadPc()
        {
            string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup");
            string settingsPath = Path.Combine(dataDir, "settings.json");

            if (!File.Exists(settingsPath))
                return null;

            using (StreamReader sr = new StreamReader(settingsPath))
            {
                string? line;

                if ((line = sr.ReadLine()) != null)
                    line = sr.ReadLine();
                else return null;

                return JsonConvert.DeserializeObject<Pc>(line!);
            }
        }

        public List<Config>? ReadConfigs()
        {
            string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FooBakCup");
            string settingsPath = Path.Combine(dataDir, "settings.json");

            if (!File.Exists(settingsPath))
                return null;

            using (StreamReader sr = new StreamReader(settingsPath))
            {
                string? line;

                for (int i = 0; i < 3; i++)
                    if ((line = sr.ReadLine()) != null) continue;
                    else return null;

                string json = sr.ReadToEnd();
                json = json.Remove(json.LastIndexOf(']'));
                return JsonConvert.DeserializeObject<List<Config>>(json);
            }
        }
    }
}
