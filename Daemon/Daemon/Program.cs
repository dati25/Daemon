using System.Net.Http.Json;
using System.Configuration;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Linq;

namespace Daemon
{
    internal class Program
    {
        static void Main(string[] args) //static async Task Main(string[] args)
        {
            //var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //var settings = configFile.AppSettings.Settings;
            //settings["PcId"].Value = null;

            //Registration registration = new Registration();

            //registration.Register();

            //HttpResponseMessage reportMessageGet = await client.GetAsync("/api/Report");
            //string reportData = await reportMessageGet.Content.ReadAsStringAsync();
            //Console.WriteLine(reportData);

            //Report report = new() {  idPC = 1, Status = false, ReportTime = DateTime.Now, Description = "Prvni report"};
            //await client.PostAsJsonAsync<Report>("/api/Report", report);

            Application app = new Application();
            app.Execute();



        }
    }
}