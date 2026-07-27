using System;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Processes;
using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
	/// <summary>
	/// Class to test conversion of Advanced IF statements.
	/// </summary>
	[TestFixture]
	public class AdvancedIfStatementTest : TestBase
	{
		TawalaProjectConverter converter;

		[SetUp]
		public void Setup()
		{
			converter = GetConverter("AdvancedIfStatements.xml");
			converter.ConvertXmlToProject();
		}

		[Test]
		public void XmlValidation()
		{
			converter = null;
			RoundtripProjectXml();
		}

		/// <summary>
		/// Tests the conversion of the XML file to a Tawala Project.
		/// </summary>
		[Test]
		public void ConvertXmlToProject()
		{
			// verify that project contains 1 form and 1 process
			Assert.AreEqual(1, Project.Current.FormList.Count);
			Assert.AreEqual(1, Project.Current.ProcessList.Count);
			Assert.AreEqual("Form 1", ((Form)Project.Current.FormList[0]).Name);
			Assert.AreEqual("Process 1", ((Process)Project.Current.ProcessList[0]).Name);

		}


		/// <summary>
		/// Tests the conversion of advanced IF statements containing a single OR operator.
		/// </summary>
		[Test]
		public void SingleOrOperator()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that process lines are correct
			Assert.AreEqual("If Q3 equals a OR Q3 equals b", process.Lines[0].ToString());
		}


		/// <summary>
		/// Tests the conversion of advanced IF statements containing a single OR operator.
		/// </summary>
		[Test]
		public void MultipleOrOperators()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that process lines are correct
			Assert.AreEqual("If Q3 equals a OR Q3 equals b OR Q3 equals c", process.Lines[3].ToString());
		}

		/// <summary>
		/// Tests the conversion of advanced IF statements containing a single AND operator.
		/// </summary>
		[Test]
		public void SingleAndOperator()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that process lines are correct
			Assert.AreEqual("If Form 1:Q1:a is not blank AND Form 1:Q2:a equals \"Q2 Response\"", process.Lines[6].ToString());
		}


		/// <summary>
		/// Tests the conversion of advanced IF statements containing a single AND operator.
		/// </summary>
		[Test]
		public void MultipleAndOperators()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			// verify that process lines are correct
			Assert.AreEqual("If Form 1:Q1:a is not blank AND Form 1:Q2:a equals \"Q2 Response\" AND Form 1:Q1:a is greater than 21", process.Lines[9].ToString());
		}

		[Test]
		public void ExpressionWithFieldSingleCondition()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			Assert.AreEqual("If Form 1:Q1:a equals Form 1:Q2:a + 1", process.Lines[12].ToString());
		}

		[Test]
		public void ExpressionWithFieldsMultipleConditions()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			Assert.AreEqual("If Form 1:Q1:a equals Form 1:Q2:a + 1 OR Form 1:Q1:a equals Form 1:Q2:a - 1", process.Lines[15].ToString());
		}

		[Test]
		public void ExpressionWithFieldsConcatenated()
		{
			Process process = (Process)Project.Current.ProcessList[0];

			Assert.AreEqual("If Form 1:Q1:a equals \"Form 1:Q2:a Form 1:Q2:a\" AND Form 1:Q1:a does not equal \"Form 1:Q2:a Form 1:Q2:a\"", process.Lines[18].ToString());
		}
	}
}
