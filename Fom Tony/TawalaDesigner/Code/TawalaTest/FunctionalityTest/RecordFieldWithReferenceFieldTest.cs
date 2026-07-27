using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace TawalaTest.FunctionalityTest
{
	/// <summary>
	/// Class for testing Record Fields that have a Reference Field member
	/// </summary>
	[TestFixture]
	public class RecordFieldWithReferenceFieldTest
	{
		private Record record;
		private Blank blank;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
			Project.NewTestProject();

			IForm form = Project.Current.AddForm();

			FibItem fibItem = new FibItem();
			form.ItemList.Add(fibItem);

			blank = fibItem.BlankList[0];

			record = new Record("Record");
		}

		[Test]
		public void Construct()
		{
			RecordField recordField = new RecordField(record, blank);
			Assert.IsNotNull(recordField.Record);
			Assert.AreEqual(blank, recordField.ReferenceField);
		}

		[Test]
		public void GetString()
		{
			RecordField recordField = new RecordField(record, blank);
			Assert.AreEqual("Record:Form 1:Q1:a", recordField.ToString());
		}

		[Test]
		public void GetStringWithAlternateLabel()
		{
			blank.AlternateLabel = "Blank One";
			RecordField recordField = new RecordField(record, blank);
			Assert.AreEqual("Record:Form 1:Blank One", recordField.ToString());
		}

		[Test]
		public void Name()
		{
			RecordField recordField = new RecordField(record, blank);
			Assert.AreEqual("Record:Q1:a", recordField.FieldName);
		}

		[Test]
		public void NameWithAlternateLabel()
		{
			blank.AlternateLabel = "Blank One";
			RecordField recordField = new RecordField(record, blank);
			Assert.AreEqual("Record:Blank One", recordField.FieldName);
		}

		[Test]
		public void FieldString()
		{
			RecordField recordField = new RecordField(record, blank);
			Assert.AreEqual("<<Record:Form 1:Q1:a>>", recordField.FieldString);
		}

		[Test]
		public void FieldStringWithAlternateLabel()
		{
			blank.AlternateLabel = "Blank One";
			RecordField recordField = new RecordField(record, blank);
			Assert.AreEqual("<<Record:Form 1:Blank One>>", recordField.FieldString);
		}
	}
}
