using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;
using Tawala.Projects;

using NUnit.Framework;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class FibBlankFollowedBySpaceCauseBadSave540
	{
		private const string Q1 = "Q1";
		private const string defaultFibStyleAtttribute = " style=\"topLabels\"";

		IForm form = null;
		FibItem fib = null;

		[SetUp]
		public void SetupTestMethod()
		{
			Util.NewTestProject();
			form = Project.Current.AddForm();
			fib = new FibItem();
			form.ItemList.Add(fib);
		}

		private static readonly string rtfTrailingSpaceAfterBlanks =
			RtfConstants.FibPrologue +
			"Text \\plain\\f0\\fs20 __________ \\par }";

		private static readonly string xmlTrailingSpaceAfterBlanks =
			"<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			XmlConstants.DefaultTabsXml +
			XmlConstants.FullBeginFont + "Text " + XmlConstants.EndFont +
            "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
			XmlConstants.FullBeginFont + " " + XmlConstants.EndFont +
			"</paragraph>" + 
			"</fib>\r\n";

		[Test]
		public void TrailingSpaceInRtfRoundtripsInXml()
		{
			fib.Rtf = rtfTrailingSpaceAfterBlanks;

			string xmlBefore = fib.ToXml(Q1);
			form.ItemList.Remove(fib);

			fib = new FibItem(new XmlElement(xmlBefore, true));
			form.ItemList.Add(fib);

			string xmlAfter = fib.ToXml(Q1);

			Assert.AreEqual(xmlTrailingSpaceAfterBlanks, xmlBefore, "Initial FIB XML doesn't match expected XML");
			Assert.AreEqual(xmlTrailingSpaceAfterBlanks, xmlAfter, "Roundtrip FIB XML doesn't match expected XML");
		}

		[Test]
		public void TrailingSpaceInRtfRoundtripsInProject()
		{
			fib.Rtf = rtfTrailingSpaceAfterBlanks;

			Util.SaveAndReloadCurrentProject();
		}

		private static readonly string rtfMultipleSpacesAroundBlanks =
			RtfConstants.FibPrologue +
			"Text \\plain\\f0\\fs20 __________   _____  _____    \\par }";

		private static readonly string xmlMultipleSpacesAroundBlanks =
			"<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			XmlConstants.DefaultTabsXml +
			XmlConstants.FullBeginFont + "Text " + XmlConstants.EndFont +
            "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
			XmlConstants.FullBeginFont + "   " + XmlConstants.EndFont +
            "<blank label=\"b\" length=\"5\" required=\"false\"></blank>" +
			XmlConstants.FullBeginFont + "  " + XmlConstants.EndFont +
            "<blank label=\"c\" length=\"5\" required=\"false\"></blank>" +
			XmlConstants.FullBeginFont + "    " + XmlConstants.EndFont +
			"</paragraph>" + 
			"</fib>\r\n";

		[Test]
		public void MultipleSpaceAroundBlanksInRtfRoundtripsInXml()
		{
			fib.Rtf = rtfMultipleSpacesAroundBlanks;

			string xmlBefore = fib.ToXml(Q1);
			Console.WriteLine(xmlBefore);
			form.ItemList.Remove(fib);

			fib = new FibItem(new XmlElement(xmlBefore, true));
			form.ItemList.Add(fib);

			string xmlAfter = fib.ToXml(Q1);

			Assert.AreEqual(xmlMultipleSpacesAroundBlanks, xmlBefore, "Initial FIB XML doesn't match expected XML");
			Assert.AreEqual(xmlMultipleSpacesAroundBlanks, xmlAfter, "Roundtrip FIB XML doesn't match expected XML");
		}

		[Test]
		public void MultipleSpaceAroundBlanksInRtfRoundtripsInProject()
		{
			fib.Rtf = rtfMultipleSpacesAroundBlanks;

			Util.SaveAndReloadCurrentProject();
		}

		private static readonly string rtfTrailingXmlCharAfterBlanks = 
			RtfConstants.FibPrologue + 
			"Text \\plain\\f0\\fs20 __________>\\par }"; 

		private static readonly string xmlTrailingXmlCharAfterBlanks = 
			"<fib label=\"Q1\"" + defaultFibStyleAtttribute + ">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			XmlConstants.DefaultTabsXml +
			XmlConstants.FullBeginFont + "Text " + XmlConstants.EndFont +
            "<blank label=\"a\" length=\"10\" required=\"false\"></blank>" +
			XmlConstants.FullBeginFont + "&gt;" + XmlConstants.EndFont + // trailing xml char
			"</paragraph>" + 
			"</fib>\r\n";

		[Test]
		public void TrailingXmlCharInRtfRoundTripsInXml()
		{
			fib.Rtf = rtfTrailingXmlCharAfterBlanks;

			string xmlBefore = fib.ToXml(Q1);
			form.ItemList.Remove(fib);

			fib = new FibItem(new XmlElement(xmlBefore, true));
			form.ItemList.Add(fib);

			string xmlAfter = fib.ToXml(Q1);

			Assert.AreEqual(xmlTrailingXmlCharAfterBlanks, xmlBefore, "Initial FIB XML doesn't match expected XML");
			Assert.AreEqual(xmlTrailingXmlCharAfterBlanks, xmlAfter, "Roundtrip FIB XML doesn't match expected XML");
		}

	}
}
