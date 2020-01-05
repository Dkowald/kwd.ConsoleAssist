using System.Collections.Generic;

namespace kwd.ConsoleAssist
{
    //todo: maybe use this as a helper singleton for interactive input data.
    // so code can just use it for interactive state.
    public interface ICommandLineArguments
    {
        void SetNext(string argLine);

        /// <summary>
        /// Assign the next arguments to execute.
        /// </summary>
        string[]? Next { get; set; }

        /// <summary>
        /// View current executing arguments.
        /// </summary>
        string[] Current { get; }

        /// <summary>
        /// History of arguments executed
        /// </summary>
        IReadOnlyCollection<string[]> History {get; }
    }
}