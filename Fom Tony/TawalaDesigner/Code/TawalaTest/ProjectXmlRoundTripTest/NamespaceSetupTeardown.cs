// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectXmlRoundTripTest
{
    [SetUpFixture]
    public class NamespaceSetupTeardown
    {
        [SetUp]
        public void BeforeTestsInNamespace()
        {
            FunctionLoader.BuildAndLoad(XmlConstants.FunctionRepositoryXml);
        }

        [TearDown]
        public void AfterTestsInNamespace()
        {
        }
    }
}