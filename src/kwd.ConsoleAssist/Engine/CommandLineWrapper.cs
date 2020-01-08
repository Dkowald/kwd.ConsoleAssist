using System;
using Microsoft.Extensions.DependencyInjection;

namespace kwd.ConsoleAssist.Engine
{
    /// <summary>
    /// Generated command line wrapper.
    /// </summary>
    public class CommandLineWrapper
    {
        /// <summary>
        /// Create CommandLineWrapper; describing settings and
        /// corresponding wrapper type.
        /// </summary>
        public CommandLineWrapper(EngineSettings settings, Type generatedWrapper)
        {
            Settings = settings;
            Wrapper = generatedWrapper;
        }

        /// <summary>
        /// Type of the application model wrapper.
        /// </summary>
        public readonly Type Wrapper;

        /// <summary>
        /// Settings used to determine wrapper.
        /// </summary>
        public readonly EngineSettings Settings;

        /// <summary>
        /// Create instance of wrapper class; using container
        /// to create application model.
        /// </summary>
        public object CreateInstance(IServiceProvider container)
        {
            var model = ActivatorUtilities.CreateInstance(container, Settings.Model);
            return ActivatorUtilities.CreateInstance(container, 
                Wrapper, container, model);
        }
    }
}