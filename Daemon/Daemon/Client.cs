using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daemon
{
	public static class Client
	{
		public static HttpClient client;
		static Client()
		{
			client = new HttpClient
			{
				BaseAddress = new Uri("http://localhost:5105")
			};
		}
		
	}
}
