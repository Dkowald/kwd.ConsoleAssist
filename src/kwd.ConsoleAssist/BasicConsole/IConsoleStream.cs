using System.Threading.Tasks;

namespace kwd.ConsoleAssist.BasicConsole
{
    /// <summary>
    /// Console output stream.
    /// </summary>
    public interface IConsoleStream
    {
        /// <summary>
        /// Write text with line-end to console.
        /// </summary>
        Task WriteLine(params string[] text);

        /// <summary>
        /// Write text to console.
        /// </summary>
        Task Write(string text);
    }
}