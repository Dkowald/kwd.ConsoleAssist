using System.Collections.Generic;

using kwd.ConsoleAssist.Configuration;
using kwd.ConsoleAssist.Engine;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace kwd.ConsoleAssist
{
    public static class HostExtensions
    {
        /// <summary>
        /// Register hosted service for command line processing
        /// </summary>
        /// <param name="builder">Host builder</param>
        /// <param name="wrapper">
        /// Use <see cref="EngineSettings"/> to create wrapper class.
        /// </param>
        /// <param name="args">Command line input</param>
        /// <returns></returns>
        public static IHostBuilder ConfigureCommandLine(this IHostBuilder builder, CommandLineWrapper wrapper, string[] args)
            => builder.ConfigureServices((ctx, svc) =>
            {
                //The processing engine.
                svc.AddSingleton(wrapper)
                    .AddSingleton<ICommandLineArguments>(new DefaultCommandLine(args))
                    .AddHostedService<CliModelEngine>();
            });

        public static IConfigurationBuilder AddUpdatableCommandLine(
            this IConfigurationBuilder configurationBuilder,
            string[] args, IDictionary<string, string> switchMappings)
            => configurationBuilder.Add(new UpdatableCommandLineSource(args, switchMappings));
    }
}
