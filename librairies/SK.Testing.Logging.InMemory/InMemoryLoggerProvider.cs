using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace SK.Testing.Logging.InMemory
{
    public class InMemoryLoggerProvider : ILoggerProvider
    {
        private readonly List<string> _output;

        public InMemoryLoggerProvider(List<string> output)
        {
            _output = output;
        }

        public ILogger CreateLogger(string categoryName)
            => new InMemoryLogger(_output, categoryName);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //nothing
            }
        }
    }
}
