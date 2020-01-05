using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kwd.ConsoleAssist.BasicConsole;

namespace kwd.Cli.Tests.TestHelpers
{
    /// <summary>
    /// A fake in-memory log of console calls for testing.
    /// </summary>
    public class TestConsole : IConsole
    {
        public LogCall Out { get; } = new LogCall();
        public LogCall Error { get; } = new LogCall();
        
        #region IConsole

        /// <inheritdoc />
        IConsoleStream IConsole.Out => Out;

        /// <inheritdoc />
        IConsoleStream IConsole.Error => Error;

        IConsoleRead IConsole.Read => throw new NotImplementedException("todo");

        /// <inheritdoc />
        public IDisposable Color(ConsoleColor font, ConsoleColor? background = null)
            => new NoOpDispose();
        #endregion
        
        public class NoOpDispose : IDisposable{ public void Dispose(){}}

        public class LogCall : IConsoleStream
        {
            public readonly List<string> Data = new List<string>();

            public IEnumerable<string> Contains(string txt) =>
                Data.Where(x => x.Contains(txt));

            /// <inheritdoc />
            public Task WriteLine(params string[] text)
            {
                foreach (var line in text)
                { Data.Add(line);}

                return Task.CompletedTask;
            }

            public Task Write(string text)
            {
                Data.Add(text);
                return Task.CompletedTask;
            }
        }
    }
}