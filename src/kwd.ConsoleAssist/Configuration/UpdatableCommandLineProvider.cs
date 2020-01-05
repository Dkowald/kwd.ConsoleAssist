using System;
using System.Collections.Generic;
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
        
        public UpdatableCommandLineProvider(IEnumerable<string> args,
            IDictionary<string, string> switchMappings = null)
        {
            _args = args;
            _switchMappings = switchMappings;
            _inner = new CommandLineConfigurationProvider(_args, _switchMappings);

            //_notifyChanged = new ConfigurationReloadToken();

        }

        /// <summary>
        /// re-load config using provided args.
        /// </summary>
        public void Update(IEnumerable<string> args)
        {
            _args = args;

            _inner = new CommandLineConfigurationProvider(args, _switchMappings);
            _inner.Load();

            //Notify the data is updated (reload).
            OnReload();
        }

        public void Loadx()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
            using (IEnumerator<string> enumerator = _args.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    string key1 = enumerator.Current;
                    int startIndex = 0;
                    if (key1.StartsWith("--"))
                        startIndex = 2;
                    else if (key1.StartsWith("-"))
                        startIndex = 1;
                    else if (key1.StartsWith("/"))
                    {
                        key1 = string.Format("--{0}", (object)key1.Substring(1));
                        startIndex = 2;
                    }
                    int length = key1.IndexOf('=');
                    string index;
                    string str1;
                    if (length < 0)
                    {
                        if (startIndex != 0)
                        {
                            if (this._switchMappings != null && this._switchMappings.TryGetValue(key1, out var str2))
                                index = str2;
                            else if (startIndex != 1)
                                index = key1.Substring(startIndex);
                            else
                                continue;
                            string current = enumerator.Current;
                            if (enumerator.MoveNext())
                                str1 = enumerator.Current;
                            else
                                continue;
                        }
                        else
                            continue;
                    }
                    else
                    {
                        string key2 = key1.Substring(0, length);
                        if (this._switchMappings != null && this._switchMappings.TryGetValue(key2, out var str2))
                        {
                            index = str2;
                        }
                        else
                        {
                            if (startIndex == 1)
                                throw new FormatException("No switch");
                            index = key1.Substring(startIndex, length - startIndex);
                        }
                        str1 = key1.Substring(length + 1);
                    }
                    dictionary[index] = str1;
                }
            }
            this.Data = (IDictionary<string, string>)dictionary;
        }

        private Dictionary<string, string> GetValidatedSwitchMappingsCopy(
          IDictionary<string, string> switchMappings)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, string> switchMapping in (IEnumerable<KeyValuePair<string, string>>)switchMappings)
            {
                if (!switchMapping.Key.StartsWith("-") && !switchMapping.Key.StartsWith("--"))
                    throw new ArgumentException("Bad switch");
                if (dictionary.ContainsKey(switchMapping.Key))
                    throw new ArgumentException("Dupe switch");
                dictionary.Add(switchMapping.Key, switchMapping.Value);
            }
            return dictionary;
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
