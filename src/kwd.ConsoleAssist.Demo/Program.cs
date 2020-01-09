using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using kwd.ConsoleAssist.Demo.App;
using kwd.ConsoleAssist.Engine;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace kwd.ConsoleAssist.Demo
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var wrapper = CreateWrapper();

            //Create generic host.
            var host = Host.CreateDefaultBuilder();

            //Use Updatable command line source.
            host.ConfigureAppConfiguration(x =>
                x.AddUpdatableCommandLine(args, new Dictionary<string, string>
                {
                    //add short-form options as normal.
                    {"-v", nameof(AppConfig.Verbose) }
                })
            );

            //use environment prefix.
            host.ConfigureAppConfiguration(cfg =>
                cfg.AddEnvironmentVariables("DEMO_")
            );

            //Add console execution engine.
            host.ConfigureCommandLine(wrapper, args);

            //Add config option to host.
            host.ConfigureServices((ctx, svc) =>
            {
                svc.AddOptions()
                    .Configure<AppConfig>(ctx.Configuration);
            });
            
            //Add application service(s)
            host.ConfigureServices( (ctx, svc) =>
            {
                svc.AddSingleton<AppSession>();
            });

            host.ConfigureServices((ctx, svc) =>
            {
                svc.AddHttpClient();
            });

            try
            {
                //Run as console.
                await host.RunConsoleAsync();
            }
            catch (OperationCanceledException)
            {
                /*eat ctl-c*/
                Environment.ExitCode = 130;
            }
        }

        private static CommandLineWrapper CreateWrapper()
        {
            var settings = new EngineSettings(typeof(MyApp),
                typeof(Program).Namespace ?? 
                throw new Exception("Missing root namespace"));
            
            #if DEBUG 
            var wrapper = settings.BuildDebug();
            #else
            var wrapper = settings.Build();
            #endif
            
            return wrapper;
        }
    }
}
