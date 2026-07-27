using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using Tawala.Projects;
using Tawala.Controls;
using NUnit.Framework;
//using NUnit.Extensions.Forms;
using Tawala.Projects.Components;
using TawalaTest.TestSupport;


namespace TawalaTest.ControlsTest
{
	[TestFixture]
	public class CheckedFormComboBoxTest
	{
		private CheckedFormComboBox comboBox;
		private IForm tawalaForm1;
		private IForm tawalaForm2;
		private IForm tawalaForm3;

		[SetUp]
		public /*override*/ void Setup()
		{
			//base.Setup();

			Util.NewTestProject();
			
			System.Windows.Forms.Form windowsForm = new System.Windows.Forms.Form();

			comboBox = new CheckedFormComboBox("testComboBox");
			windowsForm.Controls.Add(comboBox);

			FormList forms = new FormList();
			tawalaForm1 = ComponentMaker.MakeFormObject("Form 1");
			tawalaForm2 = ComponentMaker.MakeFormObject("Form 2");
			tawalaForm3 = ComponentMaker.MakeFormObject("Form 3");
			forms.Add(tawalaForm1);
			forms.Add(tawalaForm2);
			forms.Add(tawalaForm3);

			Project.Current.SetFormList(forms);

			comboBox.Forms = forms;
			comboBox.CheckedForms = forms;

			windowsForm.Show();
		}

		[Test]
		public void CheckedComboBoxContainsCheckedForms()
		{
			Assert.IsInstanceOfType(typeof(IForm), comboBox.ListItems[0]);
			Assert.AreEqual("Form 1", ((IForm)comboBox.ListItems[0]).ToString());

			Assert.IsInstanceOfType(typeof(IForm), comboBox.ListItems[1]);
			Assert.AreEqual("Form 2", ((IForm)comboBox.ListItems[1]).ToString());

			Assert.IsInstanceOfType(typeof(IForm), comboBox.ListItems[2]);
			Assert.AreEqual("Form 3", ((IForm)comboBox.ListItems[2]).ToString());
		}

		// NOTE:
        // This test test's the Setup() method, not an intrinsic behavior of the control.
        // Maybe test should be removed?
        [Test]
		public void AllCheckBoxesCheckedByDefault()
		{
			Assert.AreEqual(3, comboBox.Forms.Count);
			Assert.AreEqual(3, comboBox.CheckedForms.Count);

			//CheckedListBoxTester checkedListBoxTester = new CheckedListBoxTester("testComboBox.checkedListBox");
			//Assert.AreEqual(3, checkedListBoxTester.CheckedItems.Count);
		}

		[Test]
		public void ClickingCheckBoxesTogglesCheckedState()
		{
			//CheckedListBoxTester checkedListBoxTester = new CheckedListBoxTester("testComboBox.checkedListBox");
			//checkedListBoxTester.SetItemChecked(0, false);
			//checkedListBoxTester.SetItemChecked(2, false);

			//Assert.AreEqual(1, checkedListBoxTester.CheckedItems.Count);
			//Assert.AreEqual(1, comboBox.CheckedForms.Count);
			//Assert.AreEqual("Form 2", comboBox.CheckedForms[0].Name);

		}

		[Test]
		public void ClickingCheckBoxesModifiesCheckedFormList()
		{
			//CheckedListBoxTester checkedListBoxTester = new CheckedListBoxTester("testComboBox.checkedListBox");
			//checkedListBoxTester.SetItemChecked(1, false);

			//Assert.AreEqual(2, comboBox.CheckedForms.Count);
			//Assert.AreEqual("Form 1", comboBox.CheckedForms[0].Name);
			//Assert.AreEqual("Form 3", comboBox.CheckedForms[1].Name);
		}

		[Test]
		public void ClickingFirstAndThirdBoxesModifiesComboBoxText()
		{
			//CheckedListBoxTester checkedListBoxTester = new CheckedListBoxTester("testComboBox.checkedListBox");

			//checkedListBoxTester.SetItemChecked(1, false);
			//Assert.AreEqual("Form 1, Form 3", comboBox.Text);
		}


		[Test]
		public void ClickingSecondAndThirdBoxesModifiesComboBoxText()
		{
			//CheckedListBoxTester checkedListBoxTester = new CheckedListBoxTester("testComboBox.checkedListBox");

			//checkedListBoxTester.SetItemChecked(0, false);
			//Assert.AreEqual("Form 2, Form 3", comboBox.Text);
		}

		[Test]
		public void SettingCheckedFormModifiesCheckedState()
		{
			FormList checkedForms = new FormList();

			checkedForms.Add(tawalaForm2);
			comboBox.CheckedForms = checkedForms;

			Assert.AreEqual(3, comboBox.Forms.Count);
			Assert.AreEqual(1, comboBox.CheckedForms.Count);
		}

		[Test]
		public void AddingCheckedFormModifiesCheckedState()
		{
			FormList forms = new FormList();

			IForm tawalaForm4 = ComponentMaker.MakeFormObject("Form 4");
			forms.Add(tawalaForm4);
			comboBox.CheckedForms = forms;

			Assert.AreEqual(4, comboBox.Forms.Count);
			Assert.AreEqual(1, comboBox.CheckedForms.Count);
		}

		[Test]
		public void RemovingFormReducesFormCount()
		{
			FormList forms = new FormList();

			IForm tawalaForm4 = ComponentMaker.MakeFormObject("Form 4");
			forms.Add(tawalaForm1);
			forms.Add(tawalaForm2);
			forms.Add(tawalaForm3);
			forms.Add(tawalaForm4);
			
			comboBox.Forms = forms;

			Assert.AreEqual(4, comboBox.Forms.Count);

			forms.RemoveAt(3);
			comboBox.Forms = forms;

			Assert.AreEqual(3, comboBox.Forms.Count);
		}

		[Test]
		public void RemovingFormReducesCheckedFormsCount()
		{
			FormList forms = new FormList();

			Tawala.Projects.Form tawalaForm4 = new Tawala.Projects.Form("Form 4");
			forms.Add(tawalaForm1);
			forms.Add(tawalaForm2);
			forms.Add(tawalaForm3);
			forms.Add(tawalaForm4);

			comboBox.Forms = forms;
			comboBox.CheckedForms = forms;

			Assert.AreEqual(4, comboBox.CheckedForms.Count);

			forms.RemoveAt(3);
			comboBox.Forms = forms;
			comboBox.CheckedForms = forms;

			Assert.AreEqual(3, comboBox.CheckedForms.Count);
		}

		[Test]
		public void VerifyDefaultCheckedListBoxAppearance()
		{
			//CheckedFormComboBoxTester comboBoxTester = new CheckedFormComboBoxTester("testComboBox");
			//CheckedListBoxTester checkedListBoxTester = new CheckedListBoxTester("checkedListBox");

			//comboBoxTester.FireEvent("DropDown", new EventArgs());

			//Assert.AreEqual(comboBox.Location.X, checkedListBoxTester.Properties.Location.X);
			//Assert.AreEqual(comboBox.Bottom, checkedListBoxTester.Properties.Top);
			//Assert.AreEqual(17 * comboBox.Forms.Count - 2, checkedListBoxTester.Properties.Height);
		}

		[Test]
		public void ClickingArrowShowsCheckedListBox()
		{
			//CheckedFormComboBoxTester comboBoxTester = new CheckedFormComboBoxTester("testComboBox");
			//CheckedListBoxTester checkedListBoxTester = new CheckedListBoxTester("checkedListBox");

			//comboBoxTester.FireEvent("DropDown", new EventArgs());

			//Assert.AreEqual(true, checkedListBoxTester.Properties.Visible);
			//Assert.AreEqual(comboBox.Location.X, checkedListBoxTester.Properties.Location.X);
			//Assert.AreEqual(comboBox.Bottom, checkedListBoxTester.Properties.Top);
			//Assert.AreEqual(17 * comboBox.Forms.Count - 2, checkedListBoxTester.Properties.Height);
		}

		[Test]
		public void ChangingFocusHidesCheckedListBox()
		{
			//CheckedFormComboBoxTester comboBoxTester = new CheckedFormComboBoxTester("testComboBox");
			//CheckedListBoxTester checkedListBoxTester = new CheckedListBoxTester("checkedListBox");

			//comboBoxTester.FireEvent("DropDown", new EventArgs());
			//Assert.AreEqual(true, checkedListBoxTester.Properties.Visible);

			//comboBoxTester.Properties.Focus();
			//Assert.AreEqual(false, checkedListBoxTester.Properties.Visible);
		}
	}
}
