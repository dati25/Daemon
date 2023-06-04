using Daemon.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FluentFTP;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Daemon.Services
{
    public class FtpService
    {
		private string? fbc;
		private string? destinationPath;
		private FtpClient? client;
		private Source? source;
		private _FtpConfig _ftpConfig;
		private Config config;
		private List<string> nonFatalErrors = new();
		private readonly SnapshotService _s = new();

		public FtpService(_FtpConfig _ftpConfig, Config config)
		{
			this._ftpConfig = _ftpConfig;
			this.fbc = @"FooBakCup" + DateTime.Now.ToString("ddMMyyHHmm");
			this.config = config;
			this.destinationPath = Path.Combine(_ftpConfig.FilePath, $"config_{this.config.Id}", fbc);
		}

        public async void DoFtp(Source source, bool create = false, bool update = false)
        {
			using (this.client = new FtpClient(_ftpConfig.Host, _ftpConfig.UserName, _ftpConfig.Password))
			{
				try
				{
					client.Connect();

					this.source = source;

					var retentionBackupNumber = GetBackupNumber() - config.Retention;
					if (retentionBackupNumber >= 0)
					{
						var retentionBackupPath = Path.Combine(destinationPath, GetOldestFileInFolder());
						DeleteBackup(retentionBackupPath);
					}

					var snapshotPath = Path.Combine(SettingsConfig.SnapshotsPath, $"config_{config.Id}.txt");
					var wrongSources = this.CheckSourcesExistence(config.Sources);
					if (wrongSources.Count > 0)
						wrongSources.ForEach(source =>
						{
							nonFatalErrors.Add($"Source(Id:{source}) does not exist.");
							config.Sources.Remove(config.Sources.Where(configSource => configSource.Id == source).First());
						});

					if (File.Exists(snapshotPath))
					{
						var snaps = _s.ReadSnapshots(snapshotPath);
						Parallel.ForEach(config.Sources, source =>
						{
							var sourcePath = source.Path!;
							client.UploadFile(sourcePath, ".");
							//_fs.Copy(sourcePath, destPath, true, snaps);
						});
					}
					else
					{
						Parallel.ForEach(config.Sources, source =>
						{
							var sourcePath = source.Path!;
							client.UploadFile(sourcePath, ".");
							//_fs.Copy(sourcePath, destPath);
						});
					}

					if (config.Compress == true)
					{
						ZipFile.CreateFromDirectory(source.Path, source.Path + ".zip");
						client.UploadFile(source.Path + ".zip", ".");
						Directory.Delete(source.Path + ".zip", true);
					}
					else
					{
						this.UploadtoFTP();
					}

					if (create)
					{
						snapshotPath = Path.Combine(SettingsConfig.SnapshotsPath, $"config_{config.Id}.txt");
						

						
						if (client.DirectoryExists(SettingsConfig.SnapshotsPath))
							client.CreateDirectory(SettingsConfig.SnapshotsPath);

						if (!client.FileExists(snapshotPath))
						{
							File.Create(snapshotPath).Close();
							config.Sources!.ForEach(source => _s.AddToSnapshot(source.Path!, snapshotPath));
						}

						if (update)
						{
							File.WriteAllText(snapshotPath, string.Empty);
							config.Sources!.ForEach(source => _s.AddToSnapshot(source.Path!, snapshotPath));
						}


						AddSnapshot(config.Id)!.GetAwaiter().GetResult();
					}
					Console.WriteLine("File uploaded successfully.");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failed to upload file. Error: {ex.Message}");
				}
				finally
				{
					client.Disconnect();
				}
			}

		}
		public async Task AddSnapshot(int configId)
		{
			HttpClient _client = new() { BaseAddress = new Uri("http://localhost:5105/") };

			Settings settings = new();

			var snapshotService = new SnapshotService();

			int idPc = settings.ReadPc()!.idPc;
			string snapshot = snapshotService.ReadSnapshot(Path.Combine(SettingsConfig.SnapshotsPath, $"config_{configId}.txt"));


			var response = await _client.PutAsJsonAsync(_client.BaseAddress + $"api/Snapshot/{idPc}/{configId}", snapshot);
		}
		//public async void UploadReport(char status = 't')
		//{
		//	Client client_ = new Client();

		//	string? serializedDesciption = nonFatalErrors.Count == 0 ? null : JsonConvert.SerializeObject(nonFatalErrors);

		//	var uploaded = await client_.PostReport(this.config, status, serializedDesciption);

		//	if (!uploaded)
		//		return;//Zapsat Report do textaku
		//}
		private void UploadtoFTP()
		{
			client.CreateDirectory(destinationPath);

			client.SetWorkingDirectory(destinationPath);

			if (File.Exists(source.Path))
			{
				string destinationFileName = Path.GetFileName(source.Path); //pokud chceme extra složku ale budeme vědět co to je za složku tak použít tyhle proměnné
				client.UploadFile(source.Path, "."); //working directory = "."
			}
			else
			{
				string sourceDirectoryName = new DirectoryInfo(source.Path).Name;
				client.UploadDirectory(source.Path, ".");
			}
		}
		private int GetBackupNumber()
		{
			using (FtpClient client = new FtpClient(_ftpConfig.Host, _ftpConfig.UserName, _ftpConfig.Password))
			{
				var fileCount = 1;


					var items = client.GetListing(destinationPath);

					fileCount = items.Count();

				return fileCount;

			}


		}
		public string GetOldestFileInFolder()
		{
			FtpListItem[] listing = client.GetListing(destinationPath);

			DateTime oldestModifiedTime = DateTime.MaxValue;
			string oldestFilePath = "";

			foreach (FtpListItem item in listing)
			{
				if (item.Type == FtpObjectType.File && item.Modified < oldestModifiedTime)
				{
					oldestModifiedTime = item.Modified;
					oldestFilePath = Path.Combine(destinationPath, item.Name);
				}
			}

			return oldestFilePath;
		}

		private void DeleteBackup(string path)
		{
			var fileList = client.GetNameListing();

			if (fileList.Contains(path + ".zip"))
				client.DeleteFile(path);
			else if (fileList.Contains(path))
				client.DeleteDirectory(path);
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




	}
}
