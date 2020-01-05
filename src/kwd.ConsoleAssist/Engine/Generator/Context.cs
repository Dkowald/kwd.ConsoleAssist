using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace kwd.ConsoleAssist.Engine.Generator
{
    public class Context
    {
        public Context(SyntaxGenerator gen, CSharpCompilation compiler)
        {
            Gen = gen;
            Compiler = compiler;
        }

        public readonly SyntaxGenerator Gen;
        public readonly CSharpCompilation Compiler;

        //todo: fix for closed generics.
        public NameSyntax TypeRef(Type t)
        {
            NameSyntax typeName;

            if (t == typeof(Task))
            {typeName = (NameSyntax)Gen.IdentifierName(nameof(Task));}
            else if (t == typeof(IServiceProvider))
            {
                typeName = (NameSyntax) Gen.IdentifierName(nameof(IServiceProvider));
            }
            else if (t == typeof(ArraySegment<string>))
            {
                typeName = (NameSyntax)
                    Gen.GenericName(
                        nameof(ArraySegment<object>),
                        Gen.TypeExpression(SpecialType.System_String));
            }
            else if (t == typeof(CancellationToken))
            {
                typeName = (NameSyntax) Gen.IdentifierName(nameof(CancellationToken));
            }
            else if (t == typeof(Task<int?>))
            {
                typeName = (NameSyntax)
                    Gen.GenericName(
                        nameof(Task<object>),
                        Gen.NullableTypeExpression(
                            Gen.TypeExpression(SpecialType.System_Int32)
                            ));
            }
            else if (t.IsGenericType)
            {
                //https://www.meziantou.net/working-with-types-in-a-roslyn-analyzer.htm
                throw new NotImplementedException("no reasonable way to resolve generic");
            }
            else
            {
                typeName = (NameSyntax)Gen.TypeExpression(
                    Compiler.GetTypeByMetadataName(t.FullName) ??
                    throw new Exception($"Unresolvable type: {t.Name}")
                );
            }

            return typeName;
        }

        public SyntaxNode TypeRef<T>() => TypeRef(typeof(T));

        /// <summary>
        /// Shorthand for remaining position arguments type.
        /// ArraySegment&lt;string&gt;
        /// </summary>
        public SyntaxNode PositionArgs() => 
            TypeRef<ArraySegment<string>>();
        
        /// <summary>
        /// place holder code for todo exception.
        /// </summary>
        public SyntaxNode[] Todo()
        {
            return new []{
                Gen.ThrowStatement(
                Gen.ObjectCreationExpression(
                    TypeRef<NotImplementedException>())
                )
            };
        }
    }
}