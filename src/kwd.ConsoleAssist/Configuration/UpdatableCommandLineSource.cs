using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace kwd.ConsoleAssist.Configuration
{
    /// <summary>
    /// Command line configuration; that can be updated at run-time.
    /// </summary>
    public class UpdatableCommandLineSource : IConfigurationSource
    {
        private readonly IDictionary<string, string>? _switchMappings;
        private readonly IEnumerable<string> _args;

        /// <summary>
        /// Create new <see cref="UpdatableCommandLineSource"/>.
        /// See <see cref="UpdatableCommandLineProvider"/>.
        /// </summary>
        public UpdatableCommandLineSource(IEnumerable<string>? args, 
            IDictionary<string, string>? switchMappings)
        {
            _args = args ?? Array.Empty<string>();
            _switchMappings = switchMappings;
        }

        ///<inheritdoc />
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new UpdatableCommandLineProvider(_args, _switchMappings);
        }
    }
}