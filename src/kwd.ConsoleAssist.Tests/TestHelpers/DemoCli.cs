using System.Threading;
using System.Threading.Tasks;

using kwd.ConsoleAssist.Tests.TestHelpers.SubCommand;

namespace kwd.ConsoleAssist.Tests.TestHelpers
{
    /// <summary>
    /// A demo cli model with all the options.
    /// </summary>
    public class DemoCli
    {
        /// <summary>
        /// run action
        /// </summary>
        public void Run(){}

        /// <summary>
        /// no-arg action
        /// </summary>
        public void Action1(){}

        /// <summary>
        /// action with arg
        /// </summary>
        public void Action2(string arg1){}

        public void ActionWithCancel(CancellationToken cancel){}

        public int ActionWithInt() => 3;

        public int? ActionWithOptionalInt() => null;

        /// <summary>
        /// Sub cmd with non-di 
        /// </summary>
        /// <returns></returns>
        public Cmd1 Sub1() => new Cmd1();

        /// <summary>
        /// Sub cmd with di
        /// </summary>
        public Cmd1 Sub2(Cmd1 cmd) => cmd;

        /// <summary>
        /// Sub cmd with async
        /// </summary>
        public AsyncTests Sub3() => new AsyncTests();

        public AsyncRunCmd AsyncRun()=> new AsyncRunCmd();

        public Cmd3 CmdWithRunOverloads() => new Cmd3();

        public Cmd4 CmdWithNoRun() => new Cmd4();

        public SubCommand1 CommandInOtherNamespace() => new SubCommand1();

        public class Cmd1
        {
            /// <summary>
            /// default action
            /// </summary>
            public void Run(){ }

            /// <summary> action with arg </summary>
            public void Foo(string arg1){}

            //over load for optional
            public void Foo(string arg1, string arg2){}

            //mix in a async overload
            public Task Foo(string arg1, string arg2, string arg3) => Task.CompletedTask;

            //not support sub command with args
            //public Cmd2 Foo2(string arg1) => new Cmd2();
            //public Cmd2 Foo3(Cmd2 cmd, string arg1) => cmd;
        }

        public class AsyncTests
        {
            /// <summary>
            /// async run action
            /// </summary>
            public Task Run() => Task.CompletedTask;

            //async action with args
            public Task Action1(string arg, CancellationToken cancel) => Task.CompletedTask;

            //public Task<int> ActionWithIntReturn() => Task.FromResult(1);

            public Task<int?> ActionWithOptionalIntReturn() => Task.FromResult<int?>(1);

            public int? ActionWithOptionalIntReturn(string arg) => 1;

            /// async sub cmd
            public Task<Cmd1> Foo(Cmd1 cmd) => Task.FromResult(cmd);
        }

        public class AsyncRunCmd
        {
            public Task<int> Run(CancellationToken cancelToken)
            {
                cancelToken.ThrowIfCancellationRequested();
                return Task.FromResult(0);
            }
        }

        public class Cmd3
        {
            public void Run(){}

            public Task Run(string arg1) => Task.CompletedTask;
        }

        public class Cmd4
        {
            public void Foo(string arg){}
        }
    }
}
