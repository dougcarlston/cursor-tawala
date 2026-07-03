using System;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Common;
using System.Security;
using System.Security.Permissions;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
	[TestFixture, Ignore("FormNameBindingSource was defined in entangled Dialogs project, test disabled for now")]
	public class FormNameSourceTest
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
			TestSupport.Util.NewTestProject();
			Tawala.Projects.IForm form1 = Project.Current.AddForm();
			Tawala.Projects.IForm form2 = Project.Current.AddForm();

			testDialog = new TestDialog();

			Proxy projectBindingSourceProxy = Proxy.Construct("Dialogs", "Tawala.Dialogs.ProjectNameBindingSource", testDialog.ComboBoxProjects, null);
			Proxy.Construct("Dialogs", "Tawala.Dialogs.FormNameBindingSource", testDialog.ComboBoxForms, projectBindingSourceProxy.ProxiedObject, null);
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

				ComboBoxForms.Location = new Point(25, 75);
				ComboBoxForms.Name = "ComboBoxForms";
				ComboBoxForms.Size = new Size(100, 50);
				this.Controls.Add(ComboBoxForms);
			}

			private void InitializeComponent()
			{
			}
		}

		[Test]
		public void FormsComboBoxContainsInitialForms()
		{
			Assert.AreEqual(2, testDialog.ComboBoxForms.Items.Count);
			Assert.AreEqual("Form 1", testDialog.ComboBoxForms.Items[0].ToString());
			Assert.AreEqual("Form 2", testDialog.ComboBoxForms.Items[1].ToString());
		}

		[Test]
		public void AddedFormsAreReflectedInFormsComboBox()
		{
			IForm form3 = Project.Current.AddForm();
			form3.StartingPoint = true;
			IForm form4 = Project.Current.AddForm();

			Assert.AreEqual(4, testDialog.ComboBoxForms.Items.Count);
			Assert.AreEqual("Form 3", testDialog.ComboBoxForms.Items[2].ToString());
			Assert.AreEqual("Form 4", testDialog.ComboBoxForms.Items[3].ToString());
		}

		[Test]
		public void RemovedFormIsReflectedInFormsComboBox()
		{
			Project.Current.RemoveForm("Form 1");

			Assert.AreEqual(1, testDialog.ComboBoxForms.Items.Count);
			Assert.AreEqual("Form 2", testDialog.ComboBoxForms.Items[0].ToString());
		}

		[Test]
		public void RenamedFormIsReflectedInFormsComboBox()
		{
			Project.Current.RenameForm("Form 1", "Renamed Form");

			Assert.AreEqual(2, testDialog.ComboBoxForms.Items.Count);
			Assert.AreEqual("Renamed Form", testDialog.ComboBoxForms.Items[0].ToString());
		}

		[Test]
		public void ExternalProjectShowsOnlyStartingPointsInFormsComboBox()
		{
			Assert.AreEqual(4, testDialog.ComboBoxProjects.Items.Count);
			Assert.AreEqual("(Current Project)", testDialog.ComboBoxProjects.Items[0].ToString());

			Assert.AreEqual("TestDeployments1", testDialog.ComboBoxProjects.Items[1].ToString());
			testDialog.ComboBoxProjects.SelectedIndex = 1;
			Assert.AreEqual(1, testDialog.ComboBoxForms.Items.Count);
			Assert.AreEqual("Is a Start Point", testDialog.ComboBoxForms.Items[0].ToString());
		}
	}
}
