// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using TawalaTest.TestSupport;

namespace TawalaTest.FunctionTests
{
    [SetUpFixture]
    public class NamespaceSetupTearDown
    {
        [SetUp]
        public void FixtureSetup()
        {
            FunctionLoader.BuildAndLoad(XmlConstants.FunctionRepositoryXml);
            Assert.IsNotNull(FunctionLoader.Repository);
        }

        [TearDown]
        public void FixtureTeardown()
        {
        }
    }
}