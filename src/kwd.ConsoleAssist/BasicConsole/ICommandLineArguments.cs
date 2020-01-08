using System.Collections.Generic;

namespace kwd.ConsoleAssist.BasicConsole
{
    /// <summary>
    /// Maintain command line arguments, providing
    /// updatable input and recording history.
    /// </summary>
    public interface ICommandLineArguments
    {
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

        /// <summary>
        /// The current set of positional arguments (lower case).
        /// </summary>
        string[] PositionalArguments { get; }
    
        /// <summary>
        /// Split text into command line, and assign as Next.
        /// </summary>
        void SetNext(string argLine);

        /// <summary>
        /// Start using the Next set of arguments,
        /// move Next to current.
        /// </summary>
        void Start();

        /// <summary>
        /// Consume current set of arguments,
        /// move Current to History.
        /// </summary>
        void Finish();
    }
}