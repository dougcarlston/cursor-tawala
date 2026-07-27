using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
    [SetUpFixture]
    public class ProjectTestSetup
    {
        private IFunctionRepository functionAssembly;

        [SetUp]
        public void RunBeforeAnyTests()
        {
            CompilationInfo info = FunctionLoader.Rebuild(XmlConstants.ComponentRepository);
            functionAssembly = FunctionLoader.Load(info.Path);
        }

        [TearDown]
        public void RunAfterAnyTest()
        {
            functionAssembly = null;
        }
    }
}