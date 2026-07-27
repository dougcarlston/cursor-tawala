using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using TawalaTest.TestingSupport;

namespace TawalaTest.StoryTest
{
    [SetUpFixture]
    public class StoryTestSetup
    {
        [SetUp]
        public void RunBeforeAnyTests()
        {
            var info = FunctionLoader.Rebuild(XmlConstants.ComponentRepository);
            FunctionLoader.Load(info.Path);

            Assert.IsFalse(info.Errors.HasErrors, info.Errors.ToString());
        }

        [TearDown]
        public void RunAfterAnyTest()
        {
        }
    }
}