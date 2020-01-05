﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using kwd.ConsoleAssist.Engine.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.Extensions.DependencyModel;

namespace kwd.ConsoleAssist.Engine
{
    /// <summary>
    /// Convert app provided model to
    /// runnable class for CLI processing.
    /// </summary>
    public class CLIModelBuilder
    {
        private readonly EngineSettings _settings;
        
        readonly SyntaxGenerator Gen;
        readonly CSharpCompilation Compiler;

        private readonly Context _ctx;

        public CLIModelBuilder(EngineSettings settings)
        {
            _settings = settings;
            
            Gen = SyntaxGenerator.GetGenerator(new AdhocWorkspace(), LanguageNames.CSharp);

            var options = new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);

            Compiler = CSharpCompilation.Create(
                $"CLI_GENERATED_{_settings.Model.Name.ToUpper()}",
                options: options,
                references: GetDependencies());

            _ctx = new Context(Gen, Compiler);
        }
        
        /// <summary>
        /// Generates source file, and in-memory assembly for the wrapper type.
        /// </summary>
        public CommandLineWrapper Build()
        {
            var (code, mem) = Unit();

            if(_settings.GeneratedOutput != null)
                using (var wr = new StreamWriter(_settings.GeneratedOutput.FullName))
                { wr.Write(code.ToFullString()); }

            var rootType = Compile(mem);

            return new CommandLineWrapper(_settings, rootType);
        }

        private (CompilationUnitSyntax codeUnit, CompilationUnitSyntax memoryUnit) Unit()
        {
            var cls = new WrapperClassGenerator(_ctx).For(_settings.Model);

            var usingDecl = new[]
            {
                Gen.NamespaceImportDeclaration("System"),
                Gen.NamespaceImportDeclaration("System.Linq"),
                Gen.NamespaceImportDeclaration("System.Threading"),
                Gen.NamespaceImportDeclaration("System.Threading.Tasks")
            };

            var code = (CompilationUnitSyntax)
                Gen.CompilationUnit(usingDecl
                    .Append(Gen.NamespaceDeclaration(_settings.Model.Namespace, cls)))
                .NormalizeWhitespace();

            var mem = (CompilationUnitSyntax)
                Gen.CompilationUnit(usingDecl
                    .Append(Gen.NamespaceDeclaration(_settings.InMemoryNamespace, cls))
               );

            return (code, mem);
        }

        private Type Compile(CompilationUnitSyntax unit)
        {
            var compiler = Compiler.AddSyntaxTrees(unit.SyntaxTree);

            var wr = new MemoryStream();

            var emitResult = compiler.Emit(wr);

            if (!emitResult.Success) throw new Exception("Generator failed");

            wr.Seek(0, SeekOrigin.Begin);
            var assembly = AssemblyLoadContext.Default.LoadFromStream(wr);

            var types = assembly.ExportedTypes;

            var rootType = types.Single(x => x.Name == "_" + _settings.Model.Name);

            return rootType;
        }

        private static MetadataReference[] GetDependencies()
        {
            //https://github.com/dotnet/core/issues/2082
            var refs =
                DependencyContext.Default.CompileLibraries
                    .SelectMany(cl => cl.ResolveReferencePaths())
                    .Select(asm => MetadataReference.CreateFromFile(asm));

            return refs.Cast<MetadataReference>().ToArray();
        }
    }
}