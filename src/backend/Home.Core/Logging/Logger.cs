using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Home.Core.Logging {

    public class Logger : ILogger {

        private readonly TextWriter _writer;
        private readonly string _className;

        public Logger(TextWriter writer, string className) {
            _writer = writer;
            _className = className;
        }

        public IDisposable BeginScope<TState>(TState state) {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
            if (!IsEnabled(logLevel)) {
                return;
            }
            if (formatter is null) {
                throw new ArgumentNullException(nameof(formatter));
            }
            _writer.WriteLine($"[{logLevel}] {_className}: {formatter(state, exception)}");
        }

    }

}
