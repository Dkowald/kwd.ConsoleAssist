using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using kwd.ConsoleAssist.BasicConsole;
using kwd.ConsoleAssist.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace kwd.ConsoleAssist.Engine
{
    /// <summary>
    /// Runtime hosted service to execute command line wrapper
    /// </summary>
    public class CliModelEngine : IHostedService
    {
        private readonly IServiceProvider _cont;
        private readonly ILogger<CliModelEngine> _log;
        private readonly IHostApplicationLifetime _life;
        private readonly CommandLineWrapper _wrapper;
        private readonly DefaultCommandLine _callerConfig;
        private readonly IConsole _con;

        private readonly UpdatableCommandLineProvider? _updatableCommandLineProvider;

        public CliModelEngine(IServiceProvider cont,
            ILogger<CliModelEngine> log,
            IHostApplicationLifetime life,
            IConfiguration configRoot,
            CommandLineWrapper wrapper,
            ICommandLineArguments callerConfig,
            IConsole con)
        {
            _cont = cont;
            _log = log;
            _life = life;
            
            _wrapper = wrapper;
            
            _con = con;

            _callerConfig = (DefaultCommandLine)callerConfig;

            _updatableCommandLineProvider =
                ((IConfigurationRoot) configRoot).Providers.OfType<UpdatableCommandLineProvider>()
                .LastOrDefault();
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //execute
            while (true)
            {
                _life.ApplicationStopping.ThrowIfCancellationRequested();

                //new scope each time (get config updates).
                using var scope = _cont.CreateScope();

                var cont = scope.ServiceProvider;

                //create wrapper.
                dynamic cli = _wrapper.CreateInstance(cont);

                _callerConfig.Start();

                var result = (int?) await cli.Execute(
                                 _callerConfig.PositionalArguments(), cancellationToken)
                    ?? _wrapper.Settings.DefaultExitCode;

                _callerConfig.Finish();

                if (result.HasValue)
                {
                    Environment.ExitCode = result.Value;
                    _life.StopApplication();
                    break;
                }

                //cannot continue if no update loop.
                if (_updatableCommandLineProvider is null){
                    _log.LogWarning(
                        $"Register {nameof(UpdatableCommandLineSource)} for interactive console app");
                    break;
                }

                _callerConfig.Next ??= ReadNextInput(cancellationToken);

                _updatableCommandLineProvider.Update(_callerConfig.Next);
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private string[] ReadNextInput(CancellationToken cancel)
        {
            var input = _con.Read.PromptString(
                _wrapper.Settings.DefaultPrompt ?? "", cancel)
                .Split(null)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            return input;
        }
    }
}