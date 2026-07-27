using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using Tawala.Controls;
using Tawala.Projects;
using NUnit.Framework;
using Tawala.Projects.Components;

//using NUnit.Extensions.Forms;


namespace TawalaTest.ControlsTest
{
	[TestFixture]
	public class FormCheckBoxTest
	{
		private FormCheckBox checkBox;
		private IForm tawalaForm;

		[SetUp]
		public /*override*/ void Setup()
		{
			//base.Setup();

			System.Windows.Forms.Form windowsForm = new System.Windows.Forms.Form();
			tawalaForm = ComponentMaker.MakeFormObject("Form 1");

			checkBox = new FormCheckBox(tawalaForm);
			checkBox.Name = "testCheckBox";
			windowsForm.Controls.Add(checkBox);

			windowsForm.Show();
		}

		[Test]
		public void FormCheckBoxMaintainsFormReference()
		{
			Assert.AreSame(tawalaForm, checkBox.Form);
		}

		[Test]
		public void FormCheckBoxTextReflectsFormName()
		{
			Assert.AreEqual("Form 1", checkBox.Text);
		}

		[Test]
		public void FormCheckBoxCheckedByDefault()
		{
			Assert.IsTrue(checkBox.Checked);
		}

		[Test]
		public void ClickingFormCheckBoxTogglesCheckedState()
		{
			//Assert.IsTrue(checkBox.Checked);

			//checkBoxTester.Click();

			//Assert.IsFalse(checkBox.Checked);

			//checkBoxTester.Click();

			//Assert.IsTrue(checkBox.Checked);
		}
	}
}
