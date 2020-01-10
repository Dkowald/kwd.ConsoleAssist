using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using kwd.ConsoleAssist.BasicConsole;
using kwd.ConsoleAssist.Configuration;
using kwd.ConsoleAssist.Engine;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace kwd.ConsoleAssist
{
    /// <summary>
    /// Helpers to simplify registration
    /// </summary>
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
        public static IHostBuilder ConfigureCommandLine(this IHostBuilder builder, 
            CommandLineWrapper wrapper, string[] args)
            => builder.ConfigureServices((ctx, svc) =>
            {
                //The processing engine.
                svc.AddSingleton(wrapper)
                    .AddSingleton<ICommandLineArguments>(new DefaultCommandLineArguments(args))
                    .AddSingleton<IConsole, DefaultConsole>()
                    .AddHostedService<CliModelEngine>();
            });

        /// <summary>
        /// Add <see cref="UpdatableCommandLineSource"/> to support interactive console.
        /// </summary>
        public static IConfigurationBuilder AddUpdatableCommandLine(
            this IConfigurationBuilder configurationBuilder,
            string[] args, IDictionary<string, string> switchMappings)
            => configurationBuilder.Add(new UpdatableCommandLineSource(args, switchMappings));

        /// <summary>
        /// Run console with common exception handling
        /// </summary>
        public static async Task RunConsoleAsync(
            this IHostBuilder builder,
            ReportError operationCanceled,
            CancellationToken cancel = default)
        {
            try
            {
                await builder.RunConsoleAsync(cancel);
            }
            catch (OperationCanceledException)
            {
                Environment.ExitCode = operationCanceled.Code;
                if(operationCanceled.Message != "")
                    Console.WriteLine(operationCanceled.Message);
            }
        }

        /// <summary>
        /// Run the console; friendlier ctl-c handler
        /// </summary>
        public static async Task RunConsoleAsync(
            this HostBuilder builder,
            Action<ConsoleLifetimeOptions> configureOptions,
            ReportError operationCanceled,
            CancellationToken cancel = default)
        {
            try
            {
                await builder.RunConsoleAsync(configureOptions, cancel);
            }
            catch (OperationCanceledException)
            {
                Environment.ExitCode = operationCanceled.Code;

                if (operationCanceled.Message != "")
                    Console.WriteLine(operationCanceled.Message);
            }
        }
    }
}
