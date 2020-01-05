using System;
using Microsoft.Extensions.DependencyInjection;

namespace kwd.ConsoleAssist.Engine
{
    /// <summary>
    /// Generated command line wrapper.
    /// </summary>
    public class CommandLineWrapper
    {
        public CommandLineWrapper(EngineSettings settings, Type generatedWrapper)
        {
            Settings = settings;
            Wrapper = generatedWrapper;
        }

        public readonly Type Wrapper;

        public readonly EngineSettings Settings;

        public object CreateInstance(IServiceProvider container)
        {
            var model = ActivatorUtilities.CreateInstance(container, Settings.Model);
            return ActivatorUtilities.CreateInstance(container, 
                Wrapper, container, model);
        }
    }
}