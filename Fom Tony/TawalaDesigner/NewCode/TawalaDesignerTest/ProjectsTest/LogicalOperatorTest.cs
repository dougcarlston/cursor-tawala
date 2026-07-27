using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class LogicalOperatorTest
	{
		private Process process;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
            Util.NewTestProject();

			// create process
			process = Project.Current.AddProcess();
		}

		[Test]
		public void ConstructString()
		{
			LogicalOperator op;

			op = new LogicalOperator("and");
			Assert.AreEqual("AND", op.ToString());

			op = new LogicalOperator("AND");
			Assert.AreEqual("AND", op.ToString());
		}

		[Test]
		public void AndFromXml() 
		{
			LogicalOperator op;

			string xmlString =
				"<and>" +
				"</and>";

			IXmlElement element = new XmlElement(xmlString);

			op = new LogicalOperator(element);
			Assert.AreEqual("AND", op.ToString());

			op = new LogicalOperator(element, process.Name);
			Assert.AreEqual("AND", op.ToString());
		}

		[Test]
		public void OrFromXml()
		{
			LogicalOperator op;

			string xmlString =
				"<or>" +
				"</or>";

			IXmlElement element = new XmlElement(xmlString);
			
			op = new LogicalOperator(element);
			Assert.AreEqual("OR", op.ToString());

			op = new LogicalOperator(element, process.Name);
			Assert.AreEqual("OR", op.ToString());
		}

	}
}
