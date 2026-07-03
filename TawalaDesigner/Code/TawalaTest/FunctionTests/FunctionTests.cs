// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;

namespace TawalaTest.FunctionTests
{
    [TestFixture]
    public class FunctionTests : FunctionTestBase
    {
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            FixtureSetup();
        }

        [Test]
        public void FunctionInstancesHaveUniqueIds()
        {
            IFunctionInfoCollection functions = functionRepository.Functions;
            IFunction instance1 = functions["record-count"].CreateInstance();
            IFunction instance2 = functions["record-count"].CreateInstance();
            IFunction instance3 = functions["popular-choice-correlation-table"].CreateInstance();

            Assert.IsTrue(instance1.InstanceId != 0);
            Assert.AreNotEqual(instance1.InstanceId, instance2.InstanceId);
            Assert.AreNotEqual(instance3.InstanceId, instance2.InstanceId);
        }
    }
}