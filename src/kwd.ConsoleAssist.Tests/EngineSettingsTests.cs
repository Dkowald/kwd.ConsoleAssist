using kwd.ConsoleAssist.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace kwd.ConsoleAssist.Tests
{
    [TestClass]
    public class EngineSettingsTests
    {
        [TestMethod]
        public void Build_()
        {
            var target = TestHelper.DemoSettings;
    
            var result = target.Build();
            
            Assert.AreEqual(typeof(_DemoCli), result.Wrapper);
        }
    }
}