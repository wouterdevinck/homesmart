using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Home.Core.Logging {

    public class LoggerProvider : ILoggerProvider {

        private readonly ConcurrentDictionary<string, Logger> _loggers;

        public LoggerProvider() {
            _loggers = new ConcurrentDictionary<string, Logger>(StringComparer.Ordinal);
        }

        public ILogger CreateLogger(string categoryName) {
            return _loggers.GetOrAdd(categoryName, category => new Logger(Console.Out, category));
        }

        public void Dispose() {
            _loggers.Clear();
        }

    }

}
