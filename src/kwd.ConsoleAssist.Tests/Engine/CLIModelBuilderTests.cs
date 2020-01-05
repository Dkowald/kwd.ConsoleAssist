using System;
using kwd.Cli.Tests.TestHelpers;
using kwd.ConsoleAssist.Engine;
using kwd.ConsoleAssist.Tests.TestHelpers;
using kwd.CoreUtil.FileSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.ConsoleAssist.Tests.Engine
{
    [TestClass]
    public class CLIModelBuilderTests
    {
        [TestMethod]
        public void Build_()
        {
            var settings = TestHelper.DemoSettings;
            var target = new CLIModelBuilder(settings);

            var outFile = settings.GeneratedOutput
                          ?? throw new Exception("Expected output file");

            outFile.EnsureDelete();
            
            var genType = target.Build();

            Assert.IsNotNull(genType);
            
            outFile.Refresh();
            Assert.IsTrue(outFile.Exists);
        }
    }
}
