using System;
using Microsoft.Framework.Logging;

namespace SyncTube.Logging
{
    public class SyncLogger : ILogger
    {
        private static readonly object synLock = new object();

        private const string RED = "\u001b[31m";
        private const string YELLOW = "\u001b[33m";
        private const string GREEN = "\u001b[32m";
        private const string BLUE = "\u001b[34m";
        private const string RESET = "\u001b[m";

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            lock (synLock)
            {
                Console.Write("[" + DateTime.Now + "] ");
                switch (logLevel)
                {
                    case LogLevel.Critical:
                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(RED);
                        break;
                    case LogLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(YELLOW);
                        break;
                    case LogLevel.Information:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(GREEN);
                        break;
                }
                Console.Write(logLevel + " : ");

                if (exception != null && exception.ToString() != null && exception.ToString().Trim().Length > 0)
                {
                    Console.WriteLine(exception.Source);
                    Console.Write(exception);
                }
                else
                {
                    Console.Write(state);
                }

                Console.WriteLine(RESET);
                Console.ResetColor();
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public IDisposable BeginScopeImpl(object state)
        {
            return null;
        }
    }
}
