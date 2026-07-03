// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
    [SetUpFixture]
    public class ProjectTestSetup
    {
        protected IFunctionRepository functionRepository;

        [SetUp]
        public void RunBeforeAnyTests()
        {
            FunctionLoader.BuildAndLoad(XmlConstants.FunctionRepositoryXml);
            functionRepository = FunctionLoader.Repository;
        }

        [TearDown]
        public void RunAfterAnyTest()
        {
            functionRepository = null;
        }
    }
}