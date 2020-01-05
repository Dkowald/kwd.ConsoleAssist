using System;
using System.IO;

using kwd.ConsoleAssist.Engine;
using kwd.ConsoleAssist.Engine.Generator;
using kwd.CoreUtil.FileSystem;

namespace kwd.ConsoleAssist
{
    public class EngineSettings
    {
        public const string DefaultInMemoryNamespace = "CLI_GENERATED";

        public const string DefaultGeneratedFileExtension = ".cli.cs";

        public static DirectoryInfo GetDefaultSourceRoot(Type model) =>
            new DirectoryInfo(
                Path.Combine(
                Path.GetDirectoryName(model.Assembly.Location) ??
                throw new Exception("Cannot resolve default source root"),"../../../"));

        public EngineSettings(Type model, string projectBaseNamespace)
        :this(model, projectBaseNamespace, GetDefaultSourceRoot(model)){}

        /// <summary>
        /// Create settings using conventions.
        /// </summary>
        public EngineSettings(Type model, string projectBaseNamespace,
            DirectoryInfo projectSource)
        {
            Model = model;
            BaseNamespace = projectBaseNamespace;
            ProjectSource = projectSource;
            GeneratedOutput = SetOutputBasedOnModel(projectBaseNamespace);
        }

        /// <summary>
        /// Command line model
        /// </summary>
        public Type Model { get; }

        /// <summary>
        /// Project base namespace.
        /// </summary>
        public string BaseNamespace { get; }

        /// <summary>
        /// Root folder for source.
        /// </summary>
        public DirectoryInfo ProjectSource { get; }

        /// <summary>
        /// Generated file output.
        /// </summary>
        public FileInfo? GeneratedOutput { get; set; }

        public string InMemoryNamespace { get; set; } = DefaultInMemoryNamespace;

        /// <summary>
        /// Remap all void-return methods so they act as though
        /// they return this value. (set to non-void for utility console app)
        /// </summary>
        public int? DefaultExitCode { get; set; }

        /// <summary>
        /// A simple prompt for interactive input.
        /// For more control, inject the <see cref="ICommandLineArguments"/>.
        /// </summary>
        public string DefaultPrompt { get; set; } = ">";

        /// <summary>
        /// Use previously generated wrapper class.
        /// </summary>
        public CommandLineWrapper Build()
        {
            var name = WrapperClassGenerator.WrapperClassName(Model);

            var rootType = Model.Assembly.GetType($"{Model.Namespace}.{name}") ??
                           throw new Exception("Generated wrapper class not found");

            return new CommandLineWrapper(this, rootType);
        }

        /// <summary>
        /// Generate wrapper class.
        /// </summary>
        public CommandLineWrapper BuildDebug()
            => new CLIModelBuilder(this).Build();

        /// <summary>
        /// Assign a default file name for generated output
        /// using Model type and project root namespace
        /// </summary>
        private FileInfo SetOutputBasedOnModel(string projectRootNamespace)
        {
            var relNamespace = Model.Namespace?.Substring(projectRootNamespace.Length) ?? "";

            var nsParts = relNamespace.Split('.');

            var root = ProjectSource.GetFolder(nsParts);

            return root.GetFile(Model.Name + DefaultGeneratedFileExtension);
        }

    }
}