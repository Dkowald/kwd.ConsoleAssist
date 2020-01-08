using System;
using System.Threading;
using System.Threading.Tasks;
using kwd.ConsoleAssist.Engine.Errors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace kwd.ConsoleAssist.Engine.Generator
{
    /// <summary>
    /// Group syntax generator and compiler for code generation.
    /// </summary>
    public class Context
    {
        /// <summary>
        /// Create code generator context.
        /// </summary>
        public Context(SyntaxGenerator gen, CSharpCompilation compiler)
        {
            Gen = gen;
            Compiler = compiler;
        }

        /// <summary>
        /// Syntax generator
        /// </summary>
        public readonly SyntaxGenerator Gen;

        /// <summary>
        /// Compiler
        /// </summary>
        public readonly CSharpCompilation Compiler;

        /// <summary>
        /// Name syntax for specified type
        /// </summary>
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
                        throw new GetTypeRefError(t)
                );
            }

            return typeName;
        }

        /// <summary>
        /// Name syntax for specified type.
        /// </summary>
        public SyntaxNode TypeRef<T>() => TypeRef(typeof(T));

        /// <summary>
        /// Shorthand for remaining position arguments type.
        /// ArraySegment&lt;string&gt;
        /// </summary>
        public SyntaxNode PositionArgs() => 
            TypeRef<ArraySegment<string>>();
    }
}