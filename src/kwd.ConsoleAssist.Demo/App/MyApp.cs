using System;

using Microsoft.Extensions.Options;

namespace kwd.ConsoleAssist.Demo.App
{
    public class MyApp
    {
        private readonly AppConfig _cfg;
        
        public MyApp(IOptions<AppConfig> cfg)
        {
            _cfg = cfg.Value;
        }

        /// <summary>The default entry method for a cli model object.</summary>
        public int Run()
        {
            var text = _cfg.Verbose == true ? 
                "Hello from Console Assist " : 
                "Hello world";

            Console.WriteLine(text);

            return 0; //exit code
        }

        /// <summary>the 'version' sub command </summary>
        public int Version()
        {
            var version = GetType().Assembly.GetName().Version?.ToString() ?? "?";

            Console.WriteLine($"Version: {version}");

            return 0;
        }

        /// <summary>hand off the work to MySubCommand</summary>
        public MySubCommand Sub1(MySubCommand action) => action;

        /// <summary>Command with variable</summary>
        public int WithArg(string arg1)
        {
            Console.WriteLine($"Hello {arg1}");
            return 0;
        }

        /// <summary>Command with 2 X variable</summary>
        public int WithArg(string arg1, string arg2)
        {
            Console.WriteLine($"Hello {arg1}, {arg2}");
            return 0;
        }

        /// <summary>Add login interactive</summary>
        public MyShell Login(MyShell action) => action;

        public NuGet NuGet(NuGet model) => model;
    }
}