using System;
using kwd.ConsoleAssist.Tests.TestHelpers;

namespace kwd.ConsoleAssist.Tests
{
    public static class TestHelper
    {
        public static string ProjectNamespace =>
            typeof(TestHelper).Namespace ??
            throw new Exception("Expected top level namespace");

        public static EngineSettings DemoSettings =>
            new EngineSettings(typeof(DemoCli), ProjectNamespace);
    }
}