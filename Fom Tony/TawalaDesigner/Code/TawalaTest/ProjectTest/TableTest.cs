using System;
using NUnit.Framework;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the Table class
	/// </summary>
	[TestFixture]
	public class TableTest
	{
		private string singleRowXmlString =
			"<table indent=\"0\">" +
			"<row>" +
			"<cell width=\"5400\">" +
			"<division indent=\"0\" align=\"left\">" +
			"Cell 1" +
			"</division>" +
			"</cell>" +
			"<cell width=\"5400\">" +
			"<division indent=\"0\" align=\"left\">" +
			"Cell 2" +
			"</division>" +
			"</cell>" +
			"</row>" +
			"</table>";

		[Test]
		public void ConstructFromXml()
		{
			IXmlElement element = new XmlElement(singleRowXmlString);
			Table table = new Table(element);

			Assert.AreEqual(1, table.Rows.Count);
			Assert.AreEqual(2, table.Rows[0].Cells.Count);
			Assert.AreEqual(1, table.Rows[0].Cells[0].Divisions.Count);
			Assert.AreEqual(1, table.Rows[0].Cells[1].Divisions.Count);
		}

		private string rightCenterLeftXmlString =
			"<table indent=\"0\">" +
			"<row>" +
			"<cell width=\"3600\">" +
			"<division indent=\"0\" align=\"right\">" +
			"Right" +
			"</division>" +
			"</cell>" +
			"<cell width=\"3600\">" +
			"<division indent=\"0\" align=\"center\">" +
			"Center" +
			"</division>" +
			"</cell>" +
			"<cell width=\"3600\">" +
			"<division indent=\"0\" align=\"left\">" +
			"Left" +
			"</division>" +
			"</cell>" +
			"</row>" +
			"</table>";

		[Test]
		public void AlignmentRightCenterLeftFromXml()
		{
			IXmlElement element = new XmlElement(rightCenterLeftXmlString);
			Table table = new Table(element);

			Assert.AreEqual(1, table.Rows.Count);
			Assert.AreEqual(3, table.Rows[0].Cells.Count);

			TableCell cell1 = table.Rows[0].Cells[0];
			TableCell cell2 = table.Rows[0].Cells[1];
			TableCell cell3 = table.Rows[0].Cells[2];

			Assert.AreEqual(1, cell1.Divisions.Count);
			Assert.AreEqual("right", cell1.Divisions[0].Align);

			Assert.AreEqual(1, cell2.Divisions.Count);
			Assert.AreEqual("center", cell2.Divisions[0].Align);

			Assert.AreEqual(1, cell3.Divisions.Count);
			Assert.AreEqual("left", cell3.Divisions[0].Align);

		}

		[Test]
		public void GetXml()
		{
			IXmlElement element = new XmlElement(singleRowXmlString);
			Table table = new Table(element);
			Assert.AreEqual(singleRowXmlString, table.ToXml());
		}

		[Test]
		public void IndentedTableToRtf()
		{
			string indentedRowXmlString =
				"<table indent=\"5128\">" +
				"<row>" +
				"<cell width=\"272\">" +
				"</cell>" +
				"<cell width=\"272\">" +
				"</cell>" +
				"</row>" +
				"</table>";

			IXmlElement element = new XmlElement(indentedRowXmlString);
			Table table = new Table(element);

			string expectedRtfString =
				@"\trowd\trleft5128\clftsWidth3\clwWidth272\cellx5400\clftsWidth3\clwWidth272\cellx5672\pard\intbl \cell \cell \row";
				//@"\trowd\trleft5128\clftsWidth3\clwWidth272\cellx272\clftsWidth3\clwWidth272\cellx544\pard\intbl \cell \cell \row";

			Console.WriteLine(table.ToRtf());

			Assert.AreEqual(expectedRtfString, table.ToRtf());
		}
	}
}
