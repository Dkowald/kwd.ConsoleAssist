using System;

namespace kwd.ConsoleAssist.BasicConsole
{
    /// <summary>
    /// Injectable Console.
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// Standard output stream
        /// </summary>
        IConsoleStream Out { get; }

        /// <summary>
        /// Standard error stream
        /// </summary>
        IConsoleStream Error { get; }

        /// <summary>
        /// Prompter for user input from standard input.
        /// </summary>
        IConsoleRead Read { get; }

        /// <summary>
        /// Set console color, dispose returned value to revert.
        /// </summary>
        IDisposable Color(ConsoleColor font, ConsoleColor? background = null);
    }
}