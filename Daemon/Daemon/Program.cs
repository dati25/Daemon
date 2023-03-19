using System.Net.Http.Json;

namespace Daemon
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5105");

            HttpResponseMessage reportMessageGet = await client.GetAsync("/api/Report");
            string reportData = await reportMessageGet.Content.ReadAsStringAsync();
            Console.WriteLine(reportData);

            Report report = new() {  idPC = 1, Status = false, ReportTime = DateTime.Now, Description = "Prvni report"};
            await client.PostAsJsonAsync<Report>("/api/Report", report);

        }
    }
}