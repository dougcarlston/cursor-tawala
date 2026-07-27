using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using TawalaTest.TestingSupport;

namespace TawalaTest.DocumentDesignerTest
{
    [SetUpFixture]
    public class NamespaceSetUpTearDown
    {
        [SetUp]
        public void BeforeTestsInNamespace()
        {
            CompilationInfo info = FunctionLoader.Rebuild(XmlConstants.ComponentRepository);
            FunctionLoader.Load(info.Path);
        }

        [TearDown]
        public void AfterTestsInNamespace()
        {
        }
    }
}