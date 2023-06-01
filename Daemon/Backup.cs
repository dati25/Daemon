using Daemon.Services;
using Daemon.Models;
using System.IO.Compression;
using System.Dynamic;
using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace Daemon;
public class Backup
{
    private Config Config { get; set; }
    private Pc pc { get; set; } 
    private List<string> DestPaths { get; }
    private Client client = new();

    private List<string> nonFatalErrors = new();

    private readonly FileService _fs = new();
    private readonly SnapshotService _s = new();

    public Backup(Config config, Pc pc)
    {
        Config = config;
        this.pc = pc;
        DestPaths = config.Destinations!.Select(x => Path.Combine(x.Path!, "FooBakCup", $"config_{config.Id}")).ToList();
    }
    public void Execute()
    {
        if (this.pc.Status != 't')
        {
            Console.WriteLine("Wrong status");
            return;
        }

        switch (this.Config.Type!.ToLower())
        {
            case "full":
                this.DoBackup();
                break;

            case "diff":
                this.DoBackup(true);
                break;

            case "incr":
                this.DoBackup(true, true);
                break;
        }
    }
    private void DoBackup(bool create = false, bool update = false)
    {
        if (Config.ExpirationDate != null && DateTime.Parse(Config.ExpirationDate) < DateTime.Now) return;
        if (!Config.Status)
        {
            Console.WriteLine("Wrong status");
            return;
        }
        if (Config.Destinations == null)
        {
            this.client.PostReport(this.Config, 'f', "No assigned destinations.").GetAwaiter();
            return;
        }
        if (Config.Sources == null)
        {
            this.client.PostReport(this.Config, 'f', "No assigned sources.").GetAwaiter();
            return;
        }


        foreach (var dest in DestPaths)
        {
            int backupNumber = GetBackupNumber(dest);
            var destPath = Path.Combine(dest, "backup_" + backupNumber);
            Directory.CreateDirectory(destPath);

            var retentionBackupNumber = backupNumber - Config.Retention;
            if (retentionBackupNumber >= 0)
            {
                var retentionBackupPath = Path.Combine(dest, "backup_" + retentionBackupNumber);
                DeleteBackup(retentionBackupPath);
            }

            var snapshotPath = Path.Combine(SettingsConfig.SnapshotsPath, $"config_{Config.Id}.txt");

            //Jestli existuji sources, pokud ne, vyhodi chybovou hlasku a cestu odstrani z Configu(jenom tehle tridy, ne z dat)
            var wrongSources = this.CheckSourcesExistence(Config.Sources);
            if (wrongSources.Count > 0)
                wrongSources.ForEach(source =>
                {
                    nonFatalErrors.Add($"Source(Id:{source}) does not exist.");
                    Config.Sources.Remove(Config.Sources.Where(configSource => configSource.Id == source).First());
                });

            //Pokud neexistuje, vlezt na databazi metoda client.GetSnapshot();
            //Vrati jenom jeden, kdyztak si to predelej, at ti vraci vsechny
            if (File.Exists(snapshotPath))
            {
                var snaps = _s.ReadSnapshots(snapshotPath);
                Parallel.ForEach(Config.Sources, source =>
                {
                    var sourcePath = source.Path!;
                    _fs.Copy(sourcePath, destPath, true, snaps);
                });
            }
            else
            {
                Parallel.ForEach(Config.Sources, source =>
                {
                    var sourcePath = source.Path!;
                    _fs.Copy(sourcePath, destPath);
                });
            }

            if (Config.Compress == true)
            {
                ZipFile.CreateFromDirectory(destPath, destPath + ".zip");
                Directory.Delete(destPath, true);
            }
        }

        if (create)
        {
            var snapshotPath = Path.Combine(SettingsConfig.SnapshotsPath, $"config_{Config.Id}.txt");

            if (!Directory.Exists(SettingsConfig.SnapshotsPath))
                Directory.CreateDirectory(SettingsConfig.SnapshotsPath);

            if (!File.Exists(snapshotPath))
            {
                File.Create(snapshotPath).Close();
                Config.Sources!.ForEach(source => _s.AddToSnapshot(source.Path!, snapshotPath));
            }

            if (update)
            {
                File.WriteAllText(snapshotPath, string.Empty);
                Config.Sources!.ForEach(source => _s.AddToSnapshot(source.Path!, snapshotPath));
            }

            var client = new Client();
            client.AddSnapshot(Config.Id)!.GetAwaiter().GetResult();
        }
        this.UploadReport('t');
    }
    public async void UploadReport(char status)
    {
        Client client = new Client();

        string? serializedDesciption = this.nonFatalErrors.Count == 0 ? null : JsonConvert.SerializeObject(this.nonFatalErrors);

        status = this.nonFatalErrors.Count > 0 ? 'w' : 't';

        var uploaded = await client.PostReport(this.Config, status, serializedDesciption);

        if (!uploaded)
        {
            SettingsConfig.UploadReport = true;
        }
    }
    private int GetBackupNumber(string path)
    {
        var d = new DirectoryInfo(path);

        var fileCount = 1;
        var dirCount = 1;

        try
        {
            var items = d.GetFiles();

            Array.Sort(items, (f1, f2) => f1.LastWriteTime.CompareTo(f2.LastWriteTime));

            var name = items[^1].Name;
            var split = name.Split('_');
            var split2 = split[1].Split('.');
            var num = int.Parse(split2[0]);

            fileCount += num;
        }
        catch
        {
            // ignored
        }

        try
        {
            var items = d.GetDirectories();

            Array.Sort(items, (d1, d2) => d1.LastWriteTime.CompareTo(d2.LastWriteTime));

            var name = items[^1].Name;
            var split = name.Split('_');
            var num = int.Parse(split[1]);

            dirCount += num;
        }
        catch
        {
            // ignored
        }

        return fileCount > dirCount ? fileCount : dirCount;
    }
    private List<int> CheckSourcesExistence(List<Source> sources)
    {
        List<int> results = new();
        foreach (var source in sources)
        {
            if (!Directory.Exists(source.Path) && !File.Exists(source.Path))
                results.Add(source.Id);
        }
        return results;
    }
    private void DeleteBackup(string path)
    {
        if (File.Exists(path + ".zip"))
            File.Delete(path + ".zip");
        else if (Directory.Exists(path))
            Directory.Delete(path, true);
    }
}