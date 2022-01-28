using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace SK.Testing.Logging.InMemory
{
    public class InMemoryLogger : ILogger
    {
        private readonly List<string> _output;
        private readonly string _categoryName;

        public InMemoryLogger(List<string> output, string categoryName)
        {
            _output = output;
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
            => NoopDisposable.Instance;

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            try
            {
                _output.Add($"{_categoryName} [{eventId}] {formatter(state, exception)}");
                if (exception != null)
                    _output.Add(exception.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured during logging", e, logLevel, eventId, state, exception);
            }
        }

        private class NoopDisposable : IDisposable
        {
            public static NoopDisposable Instance = new();
            public void Dispose()
            { }
        }
    }
}
