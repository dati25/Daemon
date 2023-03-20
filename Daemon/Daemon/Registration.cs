using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Daemon
{
	public class Registration
	{
		private NetworkInterface networkInterface;
		private string MAC;
		private int idPC;

		public void Register()
		{
			// Čtení dat, přiřazení ID

			File.WriteAllText(@"C:\Users\FooBakCup\Config.fbc", idPC.ToString()); // později asi streamwriter ted jenom tohle
		}
		public void Identify() 
		{
			
			
			// metoda GetConfig
			
		}

		public bool CheckFileIntegrity()
		{
			try
			{
				this.idPC = Convert.ToInt16(File.ReadAllText(@"C:\Users\FooBakCup\Config.fbc")); // později streamreader, ktery to převede z jsonu nebo něco
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}
	}
}
