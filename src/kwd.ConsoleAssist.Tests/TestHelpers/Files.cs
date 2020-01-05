using System.IO;

namespace kwd.Cli.Tests.TestHelpers
{
    public static class Files
    {
        public static DirectoryInfo SourceRoot { get; }
            = new DirectoryInfo(
                Path.GetFullPath("../../../",
                    Path.GetDirectoryName(typeof(Files).Assembly.Location) ?? ""));
    }
}