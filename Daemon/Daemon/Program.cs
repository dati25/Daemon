using System.Net.Http.Json;

namespace Daemon
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5105");

            HttpResponseMessage message = await client.GetAsync("/api/Computer");
            string data = await message.Content.ReadAsStringAsync();
            Console.WriteLine(data);

            Computer computer = new() { Name="Davit je gay", MacAddress="ae:31:de:fe:12", IpAddress="192.168.0.1", Status=false };
            HttpResponseMessage msg = await client.PostAsJsonAsync<Computer>("/api/Computer", computer);
        }
    }
}