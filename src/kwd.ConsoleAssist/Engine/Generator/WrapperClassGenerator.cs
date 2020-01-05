using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.Extensions.DependencyInjection;

namespace kwd.ConsoleAssist.Engine.Generator
{
    /// <summary>
    /// Generate proxy classes to support cli execution
    /// </summary>
    public class WrapperClassGenerator
    {
        private readonly Context _ctx;
        readonly SyntaxGenerator Gen;
        readonly CSharpCompilation Compiler;

        /// <summary>
        /// Generated name for wrapper class.
        /// </summary>
        public static string WrapperClassName(Type type) =>"_" + type.Name;

        public WrapperClassGenerator(Context ctx){
            _ctx = ctx;
            Gen = ctx.Gen;
            Compiler = ctx.Compiler;
        }
        
        /// <summary>
        /// Wrapper around a type.
        /// </summary>
        public ClassDeclarationSyntax For(Type type)
        {
            type = type.UnwrapTask();

            var allActions = type.ActionMethods().ToArray();

            var actionMethods = allActions
                .Where(x =>
                {
                    var t = x.ReturnType.UnwrapTask();
                    return t == typeof(void) ||
                        t == typeof(int) ||
                        t == typeof(int?);
                })
                .ToArray();

            var commandMethods = allActions.Except(actionMethods)
                .ToArray();

            var commandClasses = commandMethods
                .Select(x => x.ReturnType)
                .Distinct()
                .Select(For)
                .ToArray();

            var ops = actionMethods.Select(x => ActionMethod(x.Overloads().ToArray()))
                .Union(commandMethods.Select(CommandMethod))
                .Prepend(ExecuteOp(allActions));

            var model = _ctx.TypeRef(type);

            var (ctor, fields) = ClassCtor(model);

            var result = (ClassDeclarationSyntax)Gen.ClassDeclaration(
                "_" + type.Name,
                accessibility: Accessibility.Public,
                modifiers: DeclarationModifiers.Partial,
                members: fields.Cast<SyntaxNode>()
                    .Append(ctor)
                    .Union(ops)
                    .Union(commandClasses));

            return result;
        }

        private (ConstructorDeclarationSyntax ctor, FieldDeclarationSyntax[] fields) 
            ClassCtor(NameSyntax model)
        {
            var ctorArgs = new []
            {
                (ParameterSyntax)
                Gen.ParameterDeclaration("container",
                    _ctx.TypeRef<IServiceProvider>()),

                (ParameterSyntax)
                Gen.ParameterDeclaration("model", model),
            };

            var fields = ctorArgs.Select(x =>
            {
                var field = Gen.FieldDeclaration("_" + x.Identifier.Text, x.Type,
                    accessibility: Accessibility.Private,
                    modifiers: DeclarationModifiers.ReadOnly);

                return (FieldDeclarationSyntax)field;
            }).ToArray();

            var fieldAssignments = Gen.Assign(fields, ctorArgs);
            
            var ctor = (ConstructorDeclarationSyntax) 
                Gen.ConstructorDeclaration(
                    parameters: ctorArgs,
                    accessibility: Accessibility.Public,
                    statements: fieldAssignments);

            return (ctor, fields);
        }

        private MethodDeclarationSyntax ActionMethod(MethodInfo[] ops)
        {
            //Method to switch on arg count.
            var sortedOps = ops.Select(x => new
            {
                Op = x,
                Count = x.GetParameters().Count(p => p.ParameterType != typeof(CancellationToken))
            }).OrderByDescending(x => x.Count).ToArray();

            var asyncMethod = sortedOps.Any(x => x.Op.ReturnType.IsTask());

            var name = sortedOps.First().Op.Name;
            var switcher = sortedOps.Select(x => Gen.SwitchSection(
                Gen.LiteralExpression(x.Count),
                TaskWrapCall("_model", x.Op, asyncMethod)
            ));

            switcher = switcher.Append(Gen.DefaultSwitchSection(new[]
            {
                ThrowExtraArguments()
            }));

            switcher = new[]
            {
                Gen.SwitchStatement(Gen.MemberAccessExpression("args", "Count"), switcher)
            };

            var result = (MethodDeclarationSyntax) Gen.MethodDeclaration(name,
                accessibility: Accessibility.Public,
                modifiers: asyncMethod? DeclarationModifiers.Async : DeclarationModifiers.None,
                parameters: OpArgs(),
                returnType: _ctx.TypeRef<Task<int?>>(),
                statements: switcher);

            return result;
        }

        private MethodDeclarationSyntax CommandMethod(MethodInfo op)
        {
            var returnType = op.ReturnType.UnwrapTask();

            var arg = op.GetParameters().Any() ? 
                new []{CreateFromContainer(_ctx.TypeRef(returnType))} : 
                Array.Empty<SyntaxNode>();

            var call = Gen.InvocationExpression(
                Gen.MemberAccessExpression("_model", op.Name),
                arg);

            if (op.ReturnType.IsTask())
                call = Gen.AwaitExpression(call);

            var declareModel = Gen.LocalDeclarationStatement(
                    _ctx.TypeRef(returnType), "model", call);

            var wrapper = Gen.ObjectCreationExpression(
                Gen.IdentifierName(WrapperClassName(op)),
                    Gen.IdentifierName("_container"),
                    Gen.IdentifierName("model"));

            var exec = Gen.InvocationExpression(
                Gen.MemberAccessExpression(wrapper, "Execute"),
                Gen.IdentifierName("args"),
                Gen.IdentifierName("cancel"));

            return (MethodDeclarationSyntax) Gen.MethodDeclaration(op.Name,
                OpArgs(),
                accessibility:Accessibility.Public,
                modifiers: DeclarationModifiers.Async,
                returnType: _ctx.TypeRef<Task<int?>>(),
                statements:new []
                {
                    declareModel,
                    Gen.ReturnStatement(
                        Gen.AwaitExpression(exec))
                });
        }

        private MethodDeclarationSyntax ExecuteOp(MethodInfo[] ops)
        {
            var subOps = ops.Where(x => x.Name != "Run");

            var switches = subOps.Select(x => 
                Gen.SwitchSection(Gen.LiteralExpression(x.Name.ToLower()), new []
                {
                    Gen.ReturnStatement(
                        Gen.InvocationExpression(
                            Gen.IdentifierName(x.Name),
                            Gen.InvocationExpression(
                                Gen.MemberAccessExpression("args", "Slice"),
                                Gen.LiteralExpression(1)), Gen.IdentifierName("cancel"))
                        )
                }));

            if (ops.Any(x => x.Name == "Run"))
                switches = switches.Append(
                    Gen.DefaultSwitchSection(new[] {
                        Gen.ReturnStatement(
                            Gen.InvocationExpression(
                                Gen.IdentifierName("Run"),
                                Gen.IdentifierName("args"), 
                                Gen.IdentifierName("cancel"))
                            )
                    }));
            else
                switches = switches.Append(
                    Gen.DefaultSwitchSection(ThrowExtraArguments())
                    );

            var switcher = Gen.SwitchStatement(
                Gen.InvocationExpression(Gen.MemberAccessExpression("args", "FirstOrDefault")),
                switches);

            return (MethodDeclarationSyntax) Gen.MethodDeclaration("Execute",
                accessibility: Accessibility.Public,
                parameters: OpArgs(),
                returnType: _ctx.TypeRef<Task<int?>>(),
                statements: new []{switcher});
        }
        
        /// <summary>
        /// parameters for actions and command methods.
        /// </summary>
        /// <returns></returns>
        private SyntaxNode[] OpArgs() => new[]
        {
           Gen.ParameterDeclaration("args", _ctx.PositionArgs()),
           Gen.ParameterDeclaration("cancel", _ctx.TypeRef<CancellationToken>())
        };

        private SyntaxNode CreateFromContainer(SyntaxNode targetType)
        {
            var util =
                Gen.TypeExpression(
                    Compiler.GetTypeByMetadataName(typeof(ActivatorUtilities).FullName));

            var activator = Gen.MemberAccessExpression(util,
                Gen.GenericName(nameof(ActivatorUtilities.CreateInstance), targetType)
            );

            return Gen.InvocationExpression(activator, Gen.IdentifierName("_container"));
        }

        private string WrapperClassName(MethodInfo op)
        {
            var modelType = op.ReturnType.UnwrapTask();
            if (modelType == typeof(void))
                modelType = op.DeclaringType ??
                            throw new Exception("no type for method");

            return WrapperClassName(modelType);
        }

        /// <summary>
        /// Return Task from method, or wrap to return task.
        /// </summary>
        private SyntaxNode[] TaskWrapCall(string instance, MethodInfo op, bool isAsync)
        {
            var args = op.GetParameters()
                .Select((x, idx) =>
                {
                    if(x.ParameterType == typeof(CancellationToken))
                        return Gen.IdentifierName("cancel");

                    if(x.ParameterType == typeof(string))
                        return Gen.InvocationExpression(
                            Gen.MemberAccessExpression("args", "ElementAt"),
                            Gen.LiteralExpression(idx));
                    
                    throw new Exception("unhandled parameter type");
                }).ToArray();

            var invoker = Gen.InvocationExpression(
                Gen.MemberAccessExpression(instance, op.Name), args);

            var checkCancel = Gen.InvocationExpression(
                Gen.MemberAccessExpression("cancel", "ThrowIfCancellationRequested")
            );

            if (op.ReturnType == typeof(Task<int?>) ||
                op.ReturnType == typeof(Task<int>))
                return new[]
                {
                    Gen.ReturnStatement(Gen.AwaitExpression(invoker))
                };
            
            if (op.ReturnType == typeof(Task))
                return new[]{
                    Gen.AwaitExpression(invoker),
                    Gen.ReturnStatement(Gen.NullLiteralExpression())
            };
            
            if (op.ReturnType == typeof(int) ||
                op.ReturnType == typeof(int?))
            {
                if (isAsync)
                    return new[]
                    {
                        checkCancel,
                        Gen.ReturnStatement(invoker)
                    };
                //non async method.
                return new[]
                {
                    checkCancel,
                    Gen.ReturnStatement(Gen.TaskReturn(invoker))
                };
            }

            //any thing else, just invoke and ignore return.
            return new[]
            {
                checkCancel, invoker,
                Gen.ReturnStatement(
                    isAsync? Gen.NullLiteralExpression() : Gen.TaskReturnNullInt() )
            };

        }

        private SyntaxNode ThrowExtraArguments() =>
            Gen.ThrowStatement(Gen.ObjectCreationExpression(
                Gen.IdentifierName(nameof(Exception)),
                Gen.LiteralExpression("Extra arguments found")
            ));
    }
}