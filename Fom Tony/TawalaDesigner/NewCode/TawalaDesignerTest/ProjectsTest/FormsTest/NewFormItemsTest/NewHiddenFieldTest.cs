using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest.Forms.NewFormItems
{
	[TestFixture]
	public class NewHiddenFieldTest
	{
		private IHiddenField hiddenField;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			ComponentMaker.UseNewComponents(true);
		}

		[TearDown]
		public void TearDown()
		{
			ComponentMaker.UseNewComponents(false);
			hiddenField = null;
		}

		[Test]
		public void NewHiddenFieldItemIsAnIFormItem()
		{
			hiddenField = new NewHiddenField();

			Assert.IsTrue(hiddenField is IFormItem);
		}

		[Test]
		public void CanGetDefaultXml()
		{
			hiddenField = new NewHiddenField();

			string expectedXml =
				"<field name=\"Field1\"/>" +
				Environment.NewLine;

			Assert.AreEqual(expectedXml, hiddenField.ToXml());
		}

		[Test]
		public void NameFieldNameAndAlternateLabelAreTheSame()
		{
			hiddenField = new NewHiddenField();

			Assert.AreEqual("Field1", hiddenField.Name);
			Assert.AreEqual("Field1", hiddenField.AlternateLabel);
			Assert.AreEqual("Field1", hiddenField.FieldName);
		}

		[Test]
		public void SettingAlternateLabelUpdatesNameAndFieldName()
		{
			hiddenField = new NewHiddenField();

			hiddenField.AlternateLabel = "My Field";

			Assert.AreEqual("My Field", hiddenField.Name);
			Assert.AreEqual("My Field", hiddenField.AlternateLabel);
			Assert.AreEqual("My Field", hiddenField.FieldName);
		}

		[Test]
		public void ConstructFromXml()
		{
			string xmlString =
				"<field name=\"My Field Two\"/>" +
				Environment.NewLine;

			hiddenField = new NewHiddenField(new XmlElement(xmlString));

			Assert.AreEqual(xmlString, hiddenField.ToXml());
		}
	}
}
