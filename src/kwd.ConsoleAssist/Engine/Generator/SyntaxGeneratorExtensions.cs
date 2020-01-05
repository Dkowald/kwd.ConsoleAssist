using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace kwd.ConsoleAssist.Engine.Generator
{
    public static class SyntaxGeneratorExtensions
    {
        /// <summary>Code to assign parameters to corresponding fields.</summary>
        public static IEnumerable<SyntaxNode> Assign(this SyntaxGenerator gen, 
            IEnumerable<FieldDeclarationSyntax> fields, IEnumerable<ParameterSyntax> args)
        {
            return fields.Zip(args, (f, a) =>
            {
                var assign = gen.AssignmentStatement(
                    SyntaxFactory.IdentifierName(f.Declaration.Variables.Single().Identifier),
                    SyntaxFactory.IdentifierName(a.Identifier));

                return assign;
            });
        }
        
        public static SyntaxNode MemberAccessExpression(this SyntaxGenerator gen, string who, string memberName)
            => gen.MemberAccessExpression(gen.IdentifierName(who), memberName);

        /// <summary>Task.FromResult&lt;int?&gt;(null)</summary>
        public static SyntaxNode TaskReturnNullInt(this SyntaxGenerator Gen) =>
            Gen.TaskReturn(Gen.NullLiteralExpression());

        public static SyntaxNode TaskReturn(this SyntaxGenerator Gen, SyntaxNode node) =>
            Gen.InvocationExpression(
                Gen.MemberAccessExpression(
                    Gen.IdentifierName(nameof(Task)),
                    Gen.GenericName(nameof(Task.FromResult),
                        Gen.NullableTypeExpression(Gen.TypeExpression(SpecialType.System_Int32))
                    )),
                node);

        #region Params versions
        
        /// <summary>Creates a default section for a switch statement.</summary>
        public static SyntaxNode DefaultSwitchSection(this SyntaxGenerator gen, params SyntaxNode[] statements)
            => gen.DefaultSwitchSection(statements);

        #endregion
    }
}