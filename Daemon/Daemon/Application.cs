using Daemon.Backup;

namespace Daemon
{
    public class Application
    {
        private BackupHandler handler { get; set; } = new BackupHandler();

        public void Execute()
        {
            handler.Begin();
        }
    }
}
