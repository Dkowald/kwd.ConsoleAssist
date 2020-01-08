using System;
using System.Threading;
using kwd.ConsoleAssist.BasicConsole;
using Microsoft.Extensions.Options;

namespace kwd.ConsoleAssist.Demo.App
{
    public class MyShell
    {
        private readonly AppConfig _cfg;
        private readonly AppSession _session;
        private readonly IConsole _console;
        private readonly ICommandLineArguments _args;

        //Use IOptionsSnapshot for updatable config.
        public MyShell(IOptionsSnapshot<AppConfig> cfg, AppSession session,
            IConsole console, ICommandLineArguments args)
        {
            _cfg = cfg.Value;
            _session = session;
            _console = console;
            _args = args;
        }
        
        public int? Run()
        {
            if(_session.Name == string.Empty){
                Console.WriteLine("Enter: login setName <name>");
                return null;
            }

            if(_session.Password is null)
            {
                //specify next command and return null to run it.
                _args.SetNext($"login {nameof(SetPwd)}");
                return null;
            }

            Console.WriteLine($"Hello {_session.Name}, your password is set.");

            return 0;
        }

        //void return, so engine will prompt for more data.
        public void SetName(string arg)
        {
            Console.WriteLine($"Hello {arg}");

            if (_cfg.Verbose == true)
                Console.WriteLine($"Replacing old name '{_session.Name}'");
            
            _session.Name = arg;
        }
        
        public void SetPwd(CancellationToken cancel)
        {
            //Color me blue
            using var clr = _console.Color(ConsoleColor.Blue);

            //Prompt for secret data.
            _session.Password = _console.Read.PromptSecret("Enter password: ", cancel);

            //in-code input next command (forward to another action).
            _args.SetNext("login");
        }
    }
}