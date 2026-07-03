using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;


namespace TawalaTest.BugTest
{
	/// <summary>
	/// Tests for bug 309 (Setting negative variable value causes bad save if checkbox not used).
	/// </summary>
	[TestFixture]
	public class SetNegativeValue309
	{
		private Process process;

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			process = Project.Current.AddProcess();
		}

		[Test]
		public void SettingNegativeValueProducesProperXml()
		{
			SetStatement statement = new SetStatement();
			statement.Variable = new Variable("Variable");
			statement.Expression = new Expression("-118");

			string expectedXml =
				"<set field=\"Variable\" arithmeticAsText=\"false\">\r\n" +
				"<operand value=\"-118\"/>\r\n" +
				"</set>";

			Assert.AreEqual(expectedXml, statement.ToXml());
		}

		[Test]
		public void ProjectConversionProducesProperXml()
		{
			string xmlString =
				"<project name=\"Bug309Test\" themePath=\"default\" format=\"1.4\">" +
				"<processes>" +
				"<process name=\"Process 1\">" +
				"<set field=\"Variable\" arithmeticAsText=\"false\">" +
				"<operand value=\"-118\"/>" +
				"</set>" +
				"</process>" +
				"</processes>" +
				"</project>";

			using (MemoryStream xmlStream = new MemoryStream())
			{
				byte[] xmlByteArray = System.Text.Encoding.UTF8.GetBytes(xmlString);
				xmlStream.Write(xmlByteArray, 0, xmlByteArray.Length);

				TawalaProjectConverter converter = new TawalaProjectConverter(xmlStream);
				converter.ConvertXmlToProject();
			}

			process = (Process)Project.Current.ProcessList[0];
			SetStatement statement = (SetStatement)process.Lines[0].Statement;

			string expectedXml =
				"<set field=\"Variable\" arithmeticAsText=\"false\">\r\n" +
				"<operand value=\"-118\"/>\r\n" +
				"</set>";

			Assert.AreEqual(expectedXml, statement.ToXml());
		}
	}
}
