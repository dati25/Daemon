using Daemon.Models;
using Newtonsoft.Json;

namespace Daemon
{
    internal class Program
    {
        public static async Task Main()
        {
            Register r = new Register();
            await r.RegisterPC();
        }
    }
}