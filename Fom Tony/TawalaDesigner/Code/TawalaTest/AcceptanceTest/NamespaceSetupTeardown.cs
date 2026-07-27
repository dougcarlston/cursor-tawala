// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest
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