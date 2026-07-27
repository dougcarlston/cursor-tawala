using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class CreateExtraFieldAppearsInvalid323
    {
		private IForm form;
		private Process process;

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			form = Project.Current.AddForm();
			process = Project.Current.AddProcess();
		}

		[Test]
		public void QualifiedFieldAsVariableGeneratesValidSetLine()
		{
			FormList formList = new FormList();
			formList.Add(form);

			RecordSet recordSet = new RecordSet("Record List 1", formList);
			process.Lines.Add(new GetLine(new GetStatement(recordSet)));

			Record record = new Record("Record");
			process.Lines.Add(new ForEachRecordLine(new ForEachRecordStatement(record, recordSet)));

			SetStatement setStatement = setStatementWithQualifiedField("NewVar");
			SetLine setLine = new SetLine(setStatement);

			process.Lines.Insert(2, setLine);

			setLine.Validate();

			Assert.IsTrue(setLine.IsValid);
		}

		[Test]
		public void NewQualifiedFieldCreatesVariableInConnectedProcess()
		{
			Process process2 = Project.Current.AddProcess();
            form.ConnectedPostProcess = process2;

			SetStatement setStatement = setStatementWithQualifiedField("NewVar");

			Assert.AreEqual("Set record:Form 1:NewVar to 100", setStatement.ToString());
			Assert.AreEqual(1, process2.Variables.Count);
			Assert.AreEqual("NewVar", ((Variable)process2.Variables[0]).FieldName);
		}

		[Test]
		public void ExistingQualifiedFieldDoesNotCreateVariableInConnectedProcess()
		{
			form.ItemList.Add(new FibItem());

			Process process2 = Project.Current.AddProcess();
            form.ConnectedPostProcess = process2;

			SetStatement setStatement = setStatementWithQualifiedField("Q1:a");

			Assert.AreEqual("Set record:Form 1:Q1:a to 100", setStatement.ToString());
			Assert.AreEqual(0, process2.Variables.Count);
		}

		private SetStatement setStatementWithQualifiedField(string fieldName)
		{
			SetStatement setStatement = new SetStatement(process);
			setStatement.Variable = new AssignableField("record:Form 1:" + fieldName);
			setStatement.Expression = new Expression("100");
			return setStatement;
		}
	}
}