using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using kwd.ConsoleAssist.Tests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace kwd.ConsoleAssist.Tests.Engine
{
    public class DemoCli_ByHand
    {
        readonly IServiceProvider _container;
        readonly DemoCli _model;

        public DemoCli_ByHand(IServiceProvider container, DemoCli model)
        {
            _container = (container);
            _model = (model);
        }

        public Task Execute(ArraySegment<string> args, CancellationToken cancel)
        {
            //cmd class: switches on name (or null)
            switch (args.FirstOrDefault())
            {
                //for null, no slice
                case null: return Run(args, cancel);

                case "action1": return Action1(args.Slice(1), cancel);

                case "sub1": return Sub1(args.Slice(1), cancel);

                case "sub2" : return Sub2(args.Slice(1), cancel);

                case "sub3": return Sub3(args.Slice(1), cancel);

                default: throw new Exception("bad");
            }
        }

        public Task Action1(ArraySegment<string> args, CancellationToken cancel)
        {
            //action
            switch (args.Count)
            {
                case 0: 
                    cancel.ThrowIfCancellationRequested();
                    _model.Action1();
                    return Task.CompletedTask;

                    default: throw new Exception();
            }
        }

        public Task Run(ArraySegment<string> args, CancellationToken cancel)
        {
            //action
            switch (args.Count)
            {
                case 0:
                    cancel.ThrowIfCancellationRequested();
                    _model.Run();
                    return Task.CompletedTask;

                default:throw new Exception();
            }
        }

        public Task Sub1(ArraySegment<string> args, CancellationToken cancel)
        {
            //cmd execute : new from model
             var model = _model.Sub1();
            return new _Cmd1(_container, model)
                .Execute(args, cancel);
        }

        public Task Sub2(ArraySegment<string> args, CancellationToken cancel)
        {
            //cmd exec : di model
            var model = _model.Sub2(
                ActivatorUtilities.CreateInstance<DemoCli.Cmd1>(_container));

            return new _Cmd1(_container, model).Execute(args, cancel);
        }

        public Task Sub3(ArraySegment<string> args, CancellationToken cancel)
        {
            //cmd exe
            var model = _model.Sub3();
            return new _AsyncTests(_container, model).Execute(args, cancel);
        }

        #region Cmd Classes

        public class _Cmd1
        {
            private readonly IServiceProvider _container;
            private readonly DemoCli.Cmd1 _model;

            public _Cmd1(IServiceProvider container, DemoCli.Cmd1 model)
            {
                _container = container;
                _model = model;
            }

            public Task Execute(ArraySegment<string> args, CancellationToken cancel)
            {
                //cmd class
                switch (args.FirstOrDefault())
                {
                    case "foo": return Foo(args.Slice(1), cancel);
                    case null: return Run(args, cancel);
                    default:throw new Exception();
                }
            }

            private Task Foo(ArraySegment<string> args, CancellationToken cancel)
            {
                //action
                switch (args.Count)
                {
                    case 2:
                        cancel.ThrowIfCancellationRequested();
                        _model.Foo(args.ElementAt(0), args.ElementAt(1));
                        return Task.CompletedTask;
                    case 1: 
                        cancel.ThrowIfCancellationRequested();
                        _model.Foo(args.ElementAt(0));
                        return Task.CompletedTask;

                    default: throw new Exception();
                }
            }

            private Task Run(ArraySegment<string> args, CancellationToken cancel)
            {
                //action
                switch (args.Count)
                {
                    case 0:
                        cancel.ThrowIfCancellationRequested();
                        _model.Run();
                        return Task.CompletedTask;
                    default:
                        throw new Exception();
                }
            }
        }

        public class _AsyncTests
        {
            private readonly IServiceProvider _container;
            private readonly DemoCli.AsyncTests _model;
            public _AsyncTests(IServiceProvider container, DemoCli.AsyncTests model)
            {
                _container = container;
                _model = model;
            }

            public Task Execute(ArraySegment<string> args, CancellationToken cancel)
            {
                switch (args.FirstOrDefault())
                {
                    case null: return Run(args, cancel);
                    case "action1": return Action1(args.Slice(1), cancel);

                    default : throw new Exception();
                }
            }

            private Task Run(ArraySegment<string> args, CancellationToken cancel)
            {
                //action
                switch (args.Count)
                {
                    case 0: 
                        cancel.ThrowIfCancellationRequested();
                        _model.Run();
                        return Task.CompletedTask;

                    default: throw new Exception();
                }
            }

            private Task Action1(ArraySegment<string> args, CancellationToken cancel)
            {
                //action
                switch (args.Count)
                {
                    //case 1: return _model.Action1(args.ElementAt(0));

                    case 0: return _model.Run();

                    default: throw new Exception();
                }
            }

            private async Task Foo(ArraySegment<string> args, CancellationToken cancel)
            {
                var model = await _model.Foo(ActivatorUtilities.CreateInstance<DemoCli.Cmd1>(_container));

               await new _Cmd1(_container, model).Execute(args, cancel);
            }
        }

        public class _Cmd3
        {
            private readonly IServiceProvider _container;
            private readonly DemoCli.Cmd3 _model;
            public _Cmd3(IServiceProvider container, DemoCli.Cmd3 model)
            {
                _container = container;
                _model = model;
            }

            public Task Execute(ArraySegment<string> args, CancellationToken cancel)
            {
                return args.FirstOrDefault() switch
                {
                    "SS" => Task.CompletedTask,
                    _ => Run(args, cancel)
                };
            }

            private Task Run(ArraySegment<string> args, CancellationToken cancel)
            {
                //action
                switch (args.Count)
                {
                    case 0: 
                        cancel.ThrowIfCancellationRequested();
                        _model.Run();
                        return Task.CompletedTask;

                    case 1: return _model.Run(args.ElementAt(0));

                    default: throw new Exception();
                }
            }
        }
        #endregion
    }
}