using Microsoft.Framework.Logging;

namespace SyncTube.Logging
{
    public class SyncLogProvider : ILoggerProvider
    {

        public ILogger CreateLogger(string name)
        {
            return new SyncLogger();
        }
        
        public void Dispose()
        {
        }
    }
}
