using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace kwd.ConsoleAssist.Configuration
{
    public class UpdatableCommandLineSource : IConfigurationSource
    {
        public IDictionary<string, string> SwitchMappings { get; set; }
        public IEnumerable<string> Args { get; set; }

        public UpdatableCommandLineSource(IEnumerable<string>? args, IDictionary<string, string>? switchMappings)
        {
            Args = args ?? Array.Empty<string>();
            SwitchMappings = switchMappings ?? new Dictionary<string, string>();
        }

        ///<inheritdoc />
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new UpdatableCommandLineProvider(Args, SwitchMappings);
        }
    }
}