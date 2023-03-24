using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Daemon
{
	public class Registration
	{
		private NetworkInterface networkInterface;
		private Client client = new Client();
		public int? PCId;

		public Registration()
		{
			CheckLocalID();
		}
		public async void Register()
		{
			// Čtení dat, přiřazení ID
			Console.WriteLine("How did we get here");

			HttpResponseMessage computersMessageGet = await client.httpClient.GetAsync($"/api/Computer/{PCId}");
			string computerData = await computersMessageGet.Content.ReadAsStringAsync();
			Console.WriteLine(computerData);

			

		}
		public void Identify() 
		{
			Console.WriteLine(PCId);
			
			// metoda GetConfig
			
		}

		private void CheckLocalID()
		{
			this.ReadPCId("PCId");
			if (this.PCId == null)
			{
				Identify();
				return;
			}

			Register();
		}

		private void ReadPCId(string key)
		{
			var appSettings = ConfigurationManager.AppSettings;
			this.PCId = Convert.ToInt32(appSettings[key] ?? null);
		}

		private void UploadPCId(string key, string value)
		{
			var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			var settings = configFile.AppSettings.Settings;
			if (settings[key] == null)
			{
				settings.Add(key, value);
			}
			else // pokud se id bude updatovat
			{
				settings[key].Value = value;
			}

			configFile.Save(ConfigurationSaveMode.Modified);
			ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
		}
	}
}
