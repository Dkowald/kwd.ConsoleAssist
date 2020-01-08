using System;
using Microsoft.Extensions.Options;

namespace kwd.ConsoleAssist.Demo.App
{
    public class MySubCommand
    {
        private readonly AppConfig _cfg;

        public MySubCommand(IOptions<AppConfig> cfg)
        {
            _cfg = cfg.Value;
        }

        public int Run()
        {
            if (_cfg.Verbose == true)
            {
                Console.WriteLine("Verbose is on");
            }

            Console.WriteLine($"Hello from sub action: {nameof(MySubCommand)}");

            return 0;
        }
    }
}