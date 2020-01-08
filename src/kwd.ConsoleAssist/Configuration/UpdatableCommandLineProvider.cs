using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;

namespace kwd.ConsoleAssist.Configuration
{
    /// <summary>
    /// Configuration provider for updatable command line arguments
    /// </summary>
    /// <remarks>
    /// Adds an <see cref="Update"/> method to refresh command line.
    /// Forwards to the default <see cref="CommandLineConfigurationProvider"/>
    /// for processing.
    /// https://stackoverflow.com/questions/48770143/changetoken-onchange-not-fire-on-custom-configuration-provider
    /// </remarks>
    public class UpdatableCommandLineProvider : ConfigurationProvider
    {
        private IEnumerable<string> _args;
        private readonly IDictionary<string, string> _switchMappings;

        private CommandLineConfigurationProvider _inner;
        
        /// <summary>
        /// Create new <see cref="UpdatableCommandLineProvider"/>.
        /// </summary>
        public UpdatableCommandLineProvider(IEnumerable<string> args,
            IDictionary<string, string>? switchMappings = null)
        {
            _args = args;
            _switchMappings = switchMappings ?? new Dictionary<string, string>();
            _inner = new CommandLineConfigurationProvider(_args, _switchMappings);
        }

        /// <summary>
        /// re-load config using provided args.
        /// </summary>
        public void Update(IEnumerable<string> args)
        {
            _args = args.ToArray();

            _inner = new CommandLineConfigurationProvider(_args, _switchMappings);
            _inner.Load();

            //Notify the data is updated (reload).
            OnReload();
        }

        #region Forward to inner
        /// <inheritdoc />
        public override void Load() => _inner.Load();

        /// <inheritdoc />
        public override IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
            => _inner.GetChildKeys(earlierKeys, parentPath);

        /// <inheritdoc />
        public override void Set(string key, string value) =>
            _inner.Set(key, value);

        /// <inheritdoc />
        public override bool TryGet(string key, out string value)
            => _inner.TryGet(key, out value);

        #endregion
    }
}
