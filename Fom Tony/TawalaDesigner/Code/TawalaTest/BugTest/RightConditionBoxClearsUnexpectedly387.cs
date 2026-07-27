using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using Tawala.Controls;
using Tawala.Projects;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class RightConditionBoxClearsUnexpectedly387
	{
		FieldTextBox fieldTextBox = null;
		IForm form = null;
		Blank blank = null;
		FibItem fibItem = null;
		McqItem mcItem = null;
		Variable variable = null;

		RecordField blankRecordField = null;
		RecordField mcRecordField = null;
		RecordField varRecordField = null;

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();
			form = Project.Current.AddForm();

			fibItem = new FibItem();
			form.ItemList.Add(fibItem);
			blank = fibItem.BlankList[0];

			mcItem = new McqItem();
			form.ItemList.Add(mcItem);

			variable = new Variable("Foo");

			blankRecordField = new RecordField(new Record("r1"), blank);
			mcRecordField = new RecordField(new Record("r2"), mcItem);
			varRecordField = new RecordField(new Record("r3"), variable);

			fieldTextBox = new FieldTextBox();
		}

		[TearDown]
		public void TearDown()
		{
			fieldTextBox.Dispose();
		}

		[Test]
		public void TypeChangingReturnsFalseIfEmpty()
		{
			Assert.IsFalse(fieldTextBox.TypeChanging(blank));
			Assert.IsFalse(fieldTextBox.TypeChanging(mcItem));
			Assert.IsFalse(fieldTextBox.TypeChanging(variable));
			Assert.IsFalse(fieldTextBox.TypeChanging(blankRecordField));
			Assert.IsFalse(fieldTextBox.TypeChanging(mcRecordField));
			Assert.IsFalse(fieldTextBox.TypeChanging(varRecordField));
		}


		[Test]
		public void TypeChangingDereferencesRecordFields()
		{
			fieldTextBox.Tag = blankRecordField;
			Assert.IsFalse(fieldTextBox.TypeChanging(blank));
			Assert.IsTrue(fieldTextBox.TypeChanging(mcItem));

			fieldTextBox.Tag = mcRecordField;
			Assert.IsTrue(fieldTextBox.TypeChanging(blank));
			Assert.IsFalse(fieldTextBox.TypeChanging(mcItem));

			fieldTextBox.Tag = varRecordField;
			Assert.IsTrue(fieldTextBox.TypeChanging(blankRecordField));
			Assert.IsTrue(fieldTextBox.TypeChanging(mcRecordField));
			Assert.IsFalse(fieldTextBox.TypeChanging(varRecordField));
			Assert.IsTrue(fieldTextBox.TypeChanging(blank));
			Assert.IsTrue(fieldTextBox.TypeChanging(mcItem));
			Assert.IsFalse(fieldTextBox.TypeChanging(variable));
		}
	}
}
