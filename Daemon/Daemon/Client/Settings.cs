using Daemon.Models;
using Newtonsoft.Json;

namespace Daemon.Client
{
    public class Settings
    {
        SettingsConfig sc = new SettingsConfig();

        public Pc? SavePc(Pc? pc)
        {
            if (pc == null) return ReadPc();

            if (!Directory.Exists(sc.SETTINGSDIR))
                Directory.CreateDirectory(sc.SETTINGSDIR);

            if (File.Exists(sc.PCPATH))
                File.Delete(sc.PCPATH);

            File.WriteAllText(sc.PCPATH, JsonConvert.SerializeObject(pc, Formatting.Indented));

            return pc;
        }

        public List<Config>? SaveConfigs(List<Config>? configs)
        {
            if (configs == null) return ReadConfigs();

            if (!Directory.Exists(sc.SETTINGSDIR))
                Directory.CreateDirectory(sc.SETTINGSDIR);

            if (File.Exists(sc.CONFIGSPATH))
                File.Delete(sc.CONFIGSPATH);

            List<string> jsons = new List<string>();

            configs.ForEach(c => jsons.Add(JsonConvert.SerializeObject(c, Formatting.Indented)));

            string json = string.Join(",\n", jsons);
            using (StreamWriter sw = new StreamWriter(sc.CONFIGSPATH, true))
                sw.WriteLine(json);

            return configs;
        }

        public Pc? ReadPc()
        {
            if (!File.Exists(sc.PCPATH))
                return null;

            string json;
            using (StreamReader sr = new StreamReader(sc.PCPATH))
                json = sr.ReadToEnd();

            return JsonConvert.DeserializeObject<Pc>(json);
        }

        public List<Config>? ReadConfigs()
        {
            if (!File.Exists(sc.CONFIGSPATH))
                return null;

            string json;
            using (StreamReader sr = new StreamReader(sc.CONFIGSPATH))
                json = sr.ReadToEnd();

            return JsonConvert.DeserializeObject<List<Config>>(json);
        }

        public void Update(Pc? pc, List<Config>? configs)
        {
            if (pc == null || configs == null) return;

            File.WriteAllText(sc.PCPATH, string.Empty);
            File.WriteAllText(sc.CONFIGSPATH, string.Empty);

            SavePc(pc);
            SaveConfigs(configs);
        }
    }
}
