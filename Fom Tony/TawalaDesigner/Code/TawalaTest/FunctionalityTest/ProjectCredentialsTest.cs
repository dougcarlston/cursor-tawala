// $Workfile: ProjectCredentialsTest.cs $
// $Revision: 8 $	$Date: 11/09/07 5:20p $
// Copyright © 2005 Douglas G. Carlston. All rights reserved.

using System;
using NUnit.Framework;
using Tawala.Common;
using Tawala.Projects;

namespace TawalaTest.FunctionalityTest
{
	[TestFixture]
	public class ProjectCredentialsTest
	{
		private string originalUserName;
		private string originalPassword;

		// execute this once at beginning of tests
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			// remember the real settings so that our tests don't screw things up
			originalUserName = Tawala.Common.GlobalSettings.UserName;
			originalPassword = Tawala.Common.GlobalSettings.Password;
		}

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
            TestSupport.Util.NewTestProject();
		}

		[Test]
		public void CredentialsTest()
		{
			// create an initial username / PW pair
			Tawala.Common.GlobalSettings.UserName = "Joe Blow";
			Tawala.Common.GlobalSettings.Password = "PW0001";

			Project.Current.Name = "Test Project";

			string expectedXmlStart = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
										"<request type=\"uploadProject\" protocol=\"1.0\">\r\n";

			string expectedXmlCredentials = Tawala.Common.GlobalSettings.CredentialsElement();

			string expectedXmlEnd = "<project name=\"Test Project\" themePath=\"default\" format=\"" + Project.XmlFormatVersion + "\" designerBuild=\"0\">\r\n" +
									"<pageHeader></pageHeader>" +
									"</project>\r\n" +
									"</request>\r\n";

			// Assertion
			Assert.AreEqual(expectedXmlStart + expectedXmlCredentials + expectedXmlEnd,
				Project.Current.ToXmlForUpload(expectedXmlCredentials));

			// now change the username and PW and try again
			Tawala.Common.GlobalSettings.UserName = "A cOmpletelY BoguS nAme";
			Tawala.Common.GlobalSettings.Password = "boGUs pasword";

			expectedXmlCredentials = Tawala.Common.GlobalSettings.CredentialsElement();

			// Assertion
			Assert.AreEqual(expectedXmlStart + expectedXmlCredentials + expectedXmlEnd,
				Project.Current.ToXmlForUpload(expectedXmlCredentials));
		}

		// execute this after each test method runs
		[TearDown]
		public void TearDown()
		{
		}

		// execute this once at end of all tests
		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			// restore the original settings
			Tawala.Common.GlobalSettings.UserName = originalUserName;
			Tawala.Common.GlobalSettings.Password = originalPassword;
		}
	}
}
