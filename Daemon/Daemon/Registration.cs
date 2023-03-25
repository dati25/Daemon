using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;

namespace Daemon
{
	public class Registration
	{
		private Client client = new Client();
		public int? PcId;

		public Registration()
		{
			CheckLocalID();
			
		}
		public void Register()
		{
			// Čtení dat, přiřazení ID
			this.PcId = client.GetDeserializedPcId().Result;

			this.UploadPcId("PcId", PcId.ToString());
		}
		public void Identify() 
		{
			Console.WriteLine(PcId);
			Console.WriteLine("Identify");
			// metoda GetConfig
			
		}

		private void CheckLocalID()
		{
			this.ReadPcId("PcId");
			if (this.PcId != null)
			{
				Identify();
				return;
			}

			Register();
		}

		private void ReadPcId(string key)
		{
			var appSettings = ConfigurationManager.AppSettings;
			int i;
			if (int.TryParse(appSettings[key], out i))
				PcId = i;
			else
				PcId = null;
		}

		private void UploadPcId(string key, string value)
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
