using System;
using System.Threading;

namespace kwd.ConsoleAssist.BasicConsole
{
    /// <summary>
    /// Standard console input stream.
    /// </summary>
    public interface IConsoleRead
    {
        /// <summary>
        /// A cancellable ReadKey.
        /// </summary>
        ConsoleKeyInfo ReadKey(CancellationToken cancel, bool intercept = false);

        /// <summary>
        /// Read secret data (echo a placeholder)
        /// </summary>
        string PromptSecret(string prompt, CancellationToken cancel);

        /// <summary>
        /// Read a line of text.
        /// </summary>
        string PromptString(string prompt, CancellationToken cancel);
    }
}