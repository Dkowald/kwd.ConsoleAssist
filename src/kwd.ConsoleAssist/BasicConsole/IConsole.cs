using System;

namespace kwd.ConsoleAssist.BasicConsole
{
    public interface IConsole
    {
        IConsoleStream Out { get; }
        IConsoleStream Error { get; }

        IConsoleRead Read { get; }

        IDisposable Color(ConsoleColor font, ConsoleColor? background = null);
    }
}