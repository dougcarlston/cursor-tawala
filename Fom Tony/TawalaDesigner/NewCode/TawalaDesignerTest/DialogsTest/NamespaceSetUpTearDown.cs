using System;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.DialogsTest
{
    [SetUpFixture]
    public class NamespaceSetUpTearDown
    {
        [SetUp]
        public void BeforeTestsInNamespace()
        {
            ComponentMaker.UseNewComponents(true);

//			CompilationInfo info = FunctionLoader.Rebuild(XmlConstants.ComponentRepository);
//			FunctionLoader.Load(info.Path);
        }

        [TearDown]
        public void AfterTestsInNamespace()
        {
        }
    }
}