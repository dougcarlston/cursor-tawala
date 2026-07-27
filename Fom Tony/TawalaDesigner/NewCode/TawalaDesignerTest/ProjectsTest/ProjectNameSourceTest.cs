using System;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.IO;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Common;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
	[TestFixture, Ignore("ProjectNameBindingSource was defined in entangled Dialogs project, test disabled for now")]
	public class ProjectNameSourceTest
	{
		private static readonly string userName = "aTestUserQueryDeployments";
		private static readonly string password = "JFKMSB3141";
		private TestDialog testDialog;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			string credentialsXml = Tawala.Common.GlobalSettings.CredentialsElement(userName, password);
			DeploymentInfo.Error result = DeploymentInfo.QueryServer(credentialsXml);
			Assert.IsTrue(DeploymentInfo.Error.None == result);
		}

		[SetUp]
		public void SetUp()
		{
			TestingSupport.Util.NewTestProject();
			IForm form1 = Project.Current.AddForm();
			IForm form2 = Project.Current.AddForm();

			testDialog = new TestDialog();

			Proxy.Construct("Dialogs", "Tawala.Dialogs.ProjectNameBindingSource", testDialog.ComboBoxProjects, null);
		}

		public class TestDialog : System.Windows.Forms.Form
		{
			public ComboBox ComboBoxProjects = new ComboBox();
			public ComboBox ComboBoxForms = new ComboBox();

			public TestDialog()
			{
				this.ClientSize = new Size(400, 300);
				this.Text = "TestForm";

				ComboBoxProjects.Location = new Point(25, 25);
				ComboBoxProjects.Name = "ComboBoxProjects";
				ComboBoxProjects.Size = new Size(100, 50);
				this.Controls.Add(ComboBoxProjects);
			}

			private void InitializeComponent()
			{
			}
		}

		[Test]
		public void ProjectsComboBoxContainsCurrentAndDeployedProjects()
		{
			Assert.AreEqual(4, testDialog.ComboBoxProjects.Items.Count);
			Assert.AreEqual("(Current Project)", testDialog.ComboBoxProjects.Items[0].ToString());
			Assert.AreEqual("TestDeployments1", testDialog.ComboBoxProjects.Items[1].ToString());
			Assert.AreEqual("TestDeployments2", testDialog.ComboBoxProjects.Items[2].ToString());
			Assert.AreEqual("TestDeploymentsNoStartPoints", testDialog.ComboBoxProjects.Items[3].ToString());
		}

		[Test]
		public void ProjectsComboBoxReflectsIfCurrentProjectIsDeployed()
		{
			Project.Current.Name = "TestDeployments2";
			Proxy.Construct("Dialogs", "Tawala.Dialogs.ProjectNameBindingSource", testDialog.ComboBoxProjects, null);

			Assert.AreEqual(3, testDialog.ComboBoxProjects.Items.Count);
			Assert.AreEqual("(Current Project)", testDialog.ComboBoxProjects.Items[0].ToString());
			Assert.AreEqual("TestDeployments1", testDialog.ComboBoxProjects.Items[1].ToString());
			Assert.AreEqual("TestDeploymentsNoStartPoints", testDialog.ComboBoxProjects.Items[2].ToString());
		}
	}
}
