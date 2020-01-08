using System;

using kwd.ConsoleAssist.Engine;
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
