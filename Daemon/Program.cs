using Daemon.Services;
using Quartz;
using Microsoft.Extensions.Hosting;
namespace Daemon;
internal abstract class Program
{
    public static async Task Main()
    {
        Application app = new Application();
        await app.Run();
    }
}