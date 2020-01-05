using System.Collections.Generic;
using System.Linq;

namespace kwd.ConsoleAssist.Engine
{
    public class DefaultCommandLine : ICommandLineArguments
    {
        private readonly List<string[]> _history = new List<string[]>();
        
        public DefaultCommandLine(string[] initialArgs)
        {
            Next = initialArgs;

            Current = new string[]{};
        }

        /// <summary>
        /// Move next to current.
        /// </summary>
        public void Start()
        {
            Current = Next ?? new string[]{};
            Next = null;
        }

        /// <summary>
        /// move current to history
        /// </summary>
        public void Finish()
        {
            _history.Add(Current);
        }

        public string[] PositionalArguments() =>
            Current.Where(x => !x.StartsWith('-')).ToArray();

        public void SetNext(string args)
        {
            Next = args.Split(null)
                .Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        }

        /// <inheritdoc />
        public string[]? Next { get; set; }

        /// <inheritdoc />
        public string[] Current { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<string[]> History => _history;
    }
}