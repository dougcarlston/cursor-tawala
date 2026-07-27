using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Common;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Class to test RecordField class.
	/// </summary>
	[TestFixture]
	public class RecordFieldTest
	{
		private IForm form;
		private FibItem fibItem;

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			form = Project.Current.AddForm();

			fibItem = new FibItem();
			form.ItemList.Add(fibItem);
		}

		[Test]
		public void Construct()
		{
			Record record = new Record("Record");
			RecordField recordField = new RecordField(record, fibItem);

			Assert.IsNotNull(recordField.Record);
		}

		[Test]
		public void GetString()
		{
			Record record = new Record("Record");
			RecordField recordField = new RecordField(record, fibItem.BlankList[0]);

			Assert.AreEqual("Record:Form 1:Q1:a", recordField.ToString());
		}

		[Test]
		public void Name()
		{
			Record record = new Record("Record");
			RecordField recordField = new RecordField(record, fibItem.BlankList[0]);

			Assert.AreEqual("Record:Form 1:Q1:a", recordField.QualifiedFieldName);
		}

		[Test]
		public void FieldString()
		{
			Record record = new Record("Record");
			RecordField recordField = new RecordField(record, fibItem.BlankList[0]);

			Assert.AreEqual("<<Record:Form 1:Q1:a>>", recordField.FieldString);
		}

		[Test]
		public void OperatorDataSource()
		{
			Record record = new Record("Record");

			FibItem fibItem = new FibItem();
			RecordField fibRecordField = new RecordField(record, fibItem.BlankList[0]);
			Assert.AreEqual(HybridOperator.List.DataSource, fibRecordField.OperatorDataSource);

			McqItem mcItem = new McqItem();
			RecordField mcRecordField = new RecordField(record, mcItem);
			Assert.AreEqual(MCOneOperator.List.DataSource, mcRecordField.OperatorDataSource);

			mcItem.SelectOnlyOne = false;
			Assert.AreEqual(MCManyOperator.List.DataSource, mcRecordField.OperatorDataSource);
		}

	}
}
