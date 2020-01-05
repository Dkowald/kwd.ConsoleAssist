using System;
using System.IO;
using System.Linq;
using System.Threading;
using kwd.Cli;
using kwd.ConsoleAssist.BasicConsole;
using Microsoft.Extensions.Options;

namespace kwd.ConsoleAssist.Demo.App
{
    public class Demo
    {
        private readonly AppConfig _cfg;
        private readonly IConsole _con;
        private readonly ICommandLineArguments _cmdLine;

        private string _pwd;

        public Demo(IOptionsSnapshot<AppConfig> cfg, IConsole con, ICommandLineArguments cmdLine)
        {
            _cfg = cfg.Value;
            _con = con;
            _cmdLine = cmdLine;
        }

        public int Run() => ScanFileItemSizes();

        public void Shell(CancellationToken cancel)
        {
            _con.Out.WriteLine("Cfg: " + _cfg.Echo ?? "");
            //used default prompt.
            
            //_con.Read.PromptString(">", cancel);
        }

        public void Shell2(CancellationToken cancel)
        {
            _con.Out.WriteLine("Call controlled prompt");

            string input;
            using (_con.Color(ConsoleColor.Yellow))
            {
                input = _con.Read.PromptString("Input:", cancel);
            }

            _con.Out.WriteLine("I'll use this then: " + input);
            _cmdLine.SetNext(input);
        }

        public void Pwd(CancellationToken cancel)
        {
            _pwd = _con.Read.PromptSecret("Enter Password > ", cancel);
        }

        public void Help()
        {
            var cout = _con.Out;

            using(new TempColor(ConsoleColor.Blue))
                cout.WriteLine("DEMO Command Line App");

            using(_con.H1())
                cout.WriteLine(" File system utility");

            //run
            cout.WriteLine(
                "-directory[-d]={rootDirectory}", 
                "  specify root directory");

            //help
            cout.WriteLine("",
                "help",
                "  general command help");
        }

        private int ScanFileItemSizes()
        {
            var dir = new DirectoryInfo(_cfg.Directory ?? Directory.GetCurrentDirectory());

            if (!dir.Exists)
            {
                using(_con.Error())
                    _con.Error.WriteLine("Directory not exist");

                using (_con.ErrorTxt())
                    _con.Error.WriteLine("  " + dir.FullName);

                return -1;
            }

            using (_con.H1())
                _con.Out.WriteLine(dir.FullName);

            var dirInfo = dir.EnumerateDirectories()
                .OrderBy(x => x.FullName);

            bool hasError = false;
            using(_con.Dir())
                foreach (var item in dirInfo)
                {
                    try
                    {
                        var sz = item.Size();
                        _con.Out.WriteLine(item.Name + "/ : " + sz +" bytes");
                    }
                    catch (UnauthorizedAccessException)
                    {
                        hasError = true;
                        using (_con.ErrorTxt())
                            _con.Error.WriteLine("!" + item.Name +"/");
                    }
                }

            using(_con.File())
                foreach (var item in dir.EnumerateFiles())
                {
                    _con.Out.WriteLine(item.Name + ": " + item.Length +" bytes");
                }

            if (hasError)
            {
                using (_con.Error())
                    _con.Error.WriteLine("!Error: some folders could not be read");
            }

            return 0;
        }
    }
}