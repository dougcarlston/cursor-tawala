using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.Common;

namespace TawalaTest.CommonTest
{
	[TestFixture]
	public class DeploymentInfoTest
	{
		public static readonly string UserName = "aTestUserQueryDeployments";
		public static readonly string Password = "JFKMSB3141";

		private static readonly string proj1Name = "TestDeployments1";
		private static readonly string proj2Name = "TestDeployments2";
		private static readonly string projNoneName = "TestDeploymentsNoStartPoints";

		private static readonly string urlPrefix = @"http://(?:build|dev|www)\.tawala\.com";

		[Test]
		public void QueryServer()
		{
			string credentialsXml = Tawala.Common.GlobalSettings.CredentialsElement(UserName, Password);
			DeploymentInfo.Error result = DeploymentInfo.QueryServer(credentialsXml);
			Assert.IsTrue(DeploymentInfo.Error.None == result);
			Assert.IsTrue(DeploymentInfo.LastError == DeploymentInfo.Error.None);
		

			Deployments projects = DeploymentInfo.Projects;
			Assert.AreEqual(3, projects.Count);

			Assert.IsTrue(projects.ContainsKey(proj1Name));
			Assert.IsTrue(projects.ContainsKey(proj2Name));
			Assert.IsTrue(projects.ContainsKey(projNoneName));

			StartingPoints spTest1 = projects[proj1Name];
			StartingPoints spTest2 = projects[proj2Name];
			StartingPoints spTestNone = projects[projNoneName];

			Assert.AreEqual(1, spTest1.Count);
			Assert.IsTrue(spTest1.ContainsKey("Is a Start Point"));
			Assert.IsTrue(Regex.IsMatch(spTest1["Is a Start Point"], urlPrefix));

			Assert.AreEqual(2, spTest2.Count);
			Assert.IsTrue(spTest2.ContainsKey("Form 1"));
			Assert.IsTrue(Regex.IsMatch(spTest2["Form 1"], urlPrefix));
			Assert.IsTrue(spTest2.ContainsKey("Form 3"));
			Assert.IsTrue(Regex.IsMatch(spTest2["Form 3"], urlPrefix));

			Assert.AreEqual(0, spTestNone.Count);
		}

		[Test]
		public void QueryServerBogusAccount()
		{
			string credentialsXml = Tawala.Common.GlobalSettings.CredentialsElement("bogusTest2718", "willFailAuthentication");
			DeploymentInfo.Error result = DeploymentInfo.QueryServer(credentialsXml);
			Assert.IsTrue(DeploymentInfo.Error.Authentication == result);
			Assert.IsTrue(DeploymentInfo.LastError == DeploymentInfo.Error.Authentication);
		}
	}
}
