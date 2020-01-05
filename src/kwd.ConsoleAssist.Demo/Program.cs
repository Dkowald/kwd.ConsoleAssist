using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using kwd.Cli;
using kwd.ConsoleAssist.BasicConsole;
using kwd.ConsoleAssist.Demo.App;
using kwd.ConsoleAssist.Engine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace kwd.ConsoleAssist.Demo
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder();

            host.ConfigureAppConfiguration(x =>
                x.AddUpdatableCommandLine(args, new Dictionary<string, string>
                {
                    {"-d", "Directory"},
                })
            );

            host.ConfigureCommandLine(CreateWrapper(), args);

            host.ConfigureServices((ctx, svc) =>
            {
                //application services
                svc.AddOptions()
                    .Configure<AppConfig>(ctx.Configuration);

                svc.AddSingleton<IConsole, DefaultConsole>();
            });

            try
            {
                await host.RunConsoleAsync();
            }
            catch (OperationCanceledException)
            {
                //ctl-c (or something that requested process die).
            }

            return Environment.ExitCode;
        }

        public static CommandLineWrapper CreateWrapper()
        {
            //the engine settings.
            var settings = new EngineSettings(typeof(App.Demo), "kwd.Cli.Demo");
            
            //return settings.Build();

            //Build for debug or not.
            #if DEBUG
            return settings.BuildDebug();
            #else
            retrun settings.Build();
            #endif
        }
    }
}
