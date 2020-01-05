using System.Threading.Tasks;

namespace kwd.ConsoleAssist.BasicConsole
{
    public interface IConsoleStream
    {
        Task WriteLine(params string[] text);

        Task Write(string text);
    }
}