using System.Net.Http.Json;

namespace Daemon
{
    internal class Program
    {
        static void Main(string[] args) //static async Task Main(string[] args)
		{
            Registration registration = new Registration();


            if (registration.CheckFileIntegrity() == true)
                registration.Identify();
            else
                registration.Register();


            //HttpResponseMessage reportMessageGet = await client.GetAsync("/api/Report");
            //string reportData = await reportMessageGet.Content.ReadAsStringAsync();
            //Console.WriteLine(reportData);

            //Report report = new() {  idPC = 1, Status = false, ReportTime = DateTime.Now, Description = "Prvni report"};
            //await client.PostAsJsonAsync<Report>("/api/Report", report);

        }
    }
}