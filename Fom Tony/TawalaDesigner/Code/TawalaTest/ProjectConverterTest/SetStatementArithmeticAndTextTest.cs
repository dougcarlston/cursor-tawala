using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Processes;
using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
	[TestFixture]
	public class SetStatementArithmeticAndTextTest :TestBase
	{
		private TawalaProjectConverter converter;
		private Process process;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("SetStatementsArithmeticAndText.tawala");
			converter.ConvertXmlToProject();
			process = (Process)Project.Current.ProcessList[0];
		}

		[Test]
		public void XmlValidation()
		{
			converter = null;
			RoundtripProjectXml();
		}

		[Test]
		public void ProjectElements()
		{
			Assert.AreEqual(1, Project.Current.ProcessList.Count);
			Assert.AreEqual(4, process.Lines.Count);
		}

		[Test]
		public void ArithmeticExpression()
		{
			SetStatement statement = process.Lines[0].Statement as SetStatement;
			Assert.AreEqual("number1", statement.Variable.FieldName);
			Assert.AreEqual("10 - 2", statement.Expression.ToString());
			Assert.IsFalse(statement.TreatArithmeticAsText);
		}

		[Test]
		public void ArithmeticExpressionAsText()
		{
			SetStatement statement = process.Lines[1].Statement as SetStatement;
			Assert.AreEqual("text1", statement.Variable.FieldName);
			Assert.AreEqual("10 - 2", statement.Expression.ToString());
			Assert.IsTrue(statement.TreatArithmeticAsText);
		}

		[Test]
		public void FieldInArithmeticExpression()
		{
			SetStatement statement = process.Lines[2].Statement as SetStatement;
			Assert.AreEqual("number2", statement.Variable.FieldName);
			Assert.AreEqual("<<number1>> * 5", statement.Expression.ToString());
			Assert.IsFalse(statement.TreatArithmeticAsText);
		}

		[Test]
		public void FieldInArithmeticExpressionAsText()
		{
			SetStatement statement = process.Lines[3].Statement as SetStatement;
			Assert.AreEqual("text2", statement.Variable.FieldName);
			Assert.AreEqual("<<number1>> * 5", statement.Expression.ToString());
			Assert.IsTrue(statement.TreatArithmeticAsText);
		}
	}
}
