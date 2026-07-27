// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Exceptions;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class ProjectOpenMissingDataSources531
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            RuntimeTypeResolver.Init();
            FunctionLoader.BuildAndLoad(XmlConstants.FunctionRepositoryXml);
        }

        [Test]
        public void ThrowsProjectMissingDataSourcesException()
        {
            string path = Util.GetTestFilePath("UsesMyDataSources.tawala");

            try
            {
                Project.Open(path);
                Assert.Fail("Exception not thrown");
            }
            catch (ProjectMissingDataSourcesException exception)
            {
                Assert.AreEqual(2, exception.MissingDataSourceNames.Count);
            }
            catch (Exception exception)
            {
                Assert.Fail("Unexpected exception: " + exception);
            }
        }
    }
}