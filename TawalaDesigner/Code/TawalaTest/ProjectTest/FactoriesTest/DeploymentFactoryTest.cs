using System;
using NUnit.Framework;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Deployment;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Factories;
using TawalaTest.TestSupport;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest.FactoriesTest
{
	[TestFixture]
	public class DeploymentFactoryTest
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		[Test]
		public void CanMakeStartPointFromXml()
		{
			string xmlString =
				@"<startpoint form=""Questionnaire"" " +
				@"url=""http://build.tawala.com/p/v9u1dn4cluihhpin4006/Questionnaire""/>";

			StartPoint startPoint = DeploymentFactory.MakeObject(new XmlElement(xmlString)) as StartPoint;

			Assert.AreEqual("Questionnaire", startPoint.FormName);
			Assert.AreEqual("http://build.tawala.com/p/v9u1dn4cluihhpin4006/Questionnaire", startPoint.Url);
		}

		[Test]
		public void CanMakeProjectDeploymentFromXml()
		{
			string xmlString =
				@"<deployment project=""Test Project"">" +
				@"<startpoint form=""Customize"" url=""http://build.tawala.com/p/v9u1dn4cluihhpin4006/Customize""/>" +
				@"<startpoint form=""Questionnaire"" url=""http://build.tawala.com/p/v9u1dn4cluihhpin4006/Questionnaire""/>" +
				@"<startpoint form=""Administration"" url=""http://build.tawala.com/p/v9u1dn4cluihhpin4006/Administration""/>" +
				@"</deployment>";

			ProjectDeployment deployment = DeploymentFactory.MakeObject(new XmlElement(xmlString)) as ProjectDeployment;

			Assert.AreEqual("Test Project", deployment.ProjectName);
			Assert.AreEqual(3, deployment.StartPoints.Count);
		}

		[Test]
		public void CanMakeProjectDeploymentsFromXml()
		{
			string xmlString =
				@"<deployments user=""joeuser"">" + 
				@"<deployment project=""Project One"">" + 
				@"<startpoint form=""Form 1"" url=""http://build.tawala.com/p/8ypgm8y2n6rv1ee/ilfiet5.Form+1""/>" + 
				@"</deployment>" +
				@"<deployment project=""Project Two"">" + 
				@"<startpoint form=""Form 1"" url=""http://build.tawala.com/p/vg0joiozjzriukzytern/Form+1""/>" + 
				@"</deployment>" +
				@"<deployment project=""Project Three"">" + 
				@"<startpoint form=""Customize"" url=""http://build.tawala.com/p/v9u1dn4cluihhpin4006/Customize""/>" + 
				@"<startpoint form=""Questionnaire"" url=""http://build.tawala.com/p/v9u1dn4cluihhpin4006/Questionnaire""/>" + 
				@"<startpoint form=""Administration"" url=""http://build.tawala.com/p/v9u1dn4cluihhpin4006/Administration""/>" + 
				@"</deployment>" +
				@"</deployments>";

			ProjectDeployments deployments = DeploymentFactory.MakeObject(new XmlElement(xmlString)) as ProjectDeployments;

			Assert.AreEqual("joeuser", deployments.UserName);
			Assert.AreEqual(3, deployments.Count);
			Assert.AreEqual("Project One", deployments[0].ProjectName);
			Assert.AreEqual("Project Two", deployments[1].ProjectName);
			Assert.AreEqual("Project Three", deployments[2].ProjectName);
		}

		[Test]
		public void CanMakeDeploymentResponseFromXml()
		{
			string xmlString =
				@"<response status=""success"">" +
				@"<deployments user=""joeuser"">" +
				@"<deployment project=""Project One"">" +
				@"<startpoint form=""Form 1"" url=""http://build.tawala.com/p/8ypgm8y2n6rv1ee/ilfiet5.Form+1""/>" +
				@"</deployment>" +
				@"<deployment project=""Project Two"">" +
				@"<startpoint form=""Form 1"" url=""http://build.tawala.com/p/vg0joiozjzriukzytern/Form+1""/>" +
				@"</deployment>" +
				@"<deployment project=""Project Three"">" +
				@"<startpoint form=""Customize"" url=""http://build.tawala.com/p/v9u1dn4cluihhpin4006/Customize""/>" +
				@"<startpoint form=""Questionnaire"" url=""http://build.tawala.com/p/v9u1dn4cluihhpin4006/Questionnaire""/>" +
				@"<startpoint form=""Administration"" url=""http://build.tawala.com/p/v9u1dn4cluihhpin4006/Administration""/>" +
				@"</deployment>" +
				@"</deployments>" +
				@"</response>";

			DeploymentResponse response = DeploymentFactory.MakeObject(new XmlElement(xmlString)) as DeploymentResponse;

			Assert.AreEqual("success", response.Status);
			Assert.AreEqual(3, response.Deployments.Count);
		}

		[Test]
		public void DeploymentResponseReturnsStartPointsBasedOnProjectNames()
		{
			string xmlString =
				@"<response status=""success"">" +
				@"<deployments user=""joeuser"">" +
				@"<deployment project=""Project One"">" +
				@"<startpoint form=""Form 1"" url=""http://build.tawala.com/p/8ypgm8y2n6rv1ee/ilfiet5.Form+1""/>" +
				@"</deployment>" +
				@"<deployment project=""Project Two"">" +
				@"<startpoint form=""Form 1"" url=""http://build.tawala.com/p/vg0joiozjzriukzytern/Form+1""/>" +
				@"</deployment>" +
				@"<deployment project=""Project Three"">" +
				@"<startpoint form=""Customize"" url=""http://build.tawala.com/p/v9u1dn4cluihhpin4006/Customize""/>" +
				@"<startpoint form=""Questionnaire"" url=""http://build.tawala.com/p/v9u1dn4cluihhpin4006/Questionnaire""/>" +
				@"<startpoint form=""Administration"" url=""http://build.tawala.com/p/v9u1dn4cluihhpin4006/Administration""/>" +
				@"</deployment>" +
				@"</deployments>" +
				@"</response>";

			IDeploymentResponse response = DeploymentFactory.MakeObject(new XmlElement(xmlString)) as IDeploymentResponse;

			Assert.AreEqual(1, response.GetStartPoints("Project One").Count);
			Assert.AreEqual("Form 1", response.GetStartPoints("Project One")[0].FormName);

			Assert.AreEqual(1, response.GetStartPoints("Project Two").Count);
			Assert.AreEqual("Form 1", response.GetStartPoints("Project Two")[0].FormName);

			Assert.AreEqual(3, response.GetStartPoints("Project Three").Count);
			Assert.AreEqual("Customize", response.GetStartPoints("Project Three")[0].FormName);
			Assert.AreEqual("Questionnaire", response.GetStartPoints("Project Three")[1].FormName);
			Assert.AreEqual("Administration", response.GetStartPoints("Project Three")[2].FormName);
		}

	}
}
