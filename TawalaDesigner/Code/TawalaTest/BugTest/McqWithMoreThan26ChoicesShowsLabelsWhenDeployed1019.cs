using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
	/// <summary>
	/// Test for Mantis issue 1019 (If an MCQ has more than 26 choices, the letters at the beginning of the choice become visible in the MCQ ).
	/// </summary>
	[TestFixture]
	public class McqWithMoreThan26ChoicesShowsLabelsWhenDeployed1019
	{
		private static readonly string stdElementsOpen =
			@"<paragraph indent=""0"" align=""left"">" +
			XmlConstants.DefaultTabsXml +
			XmlConstants.FullBeginFont;

		private static readonly string stdElementsClose = XmlConstants.EndFont + @"</paragraph>";

		private static readonly string mcqXmlString =
			@"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""vertical"">" +
			@"<question>" + stdElementsOpen + @"Choose:" + stdElementsClose + @"</question>" +
			@"<choice label=""a"">" + stdElementsOpen + @"choice 1" + stdElementsClose + @"</choice>" +
			@"<choice label=""b"">" + stdElementsOpen + @"choice 2" + stdElementsClose + @"</choice>" +
			@"<choice label=""c"">" + stdElementsOpen + @"choice 3" + stdElementsClose + @"</choice>" +
			@"<choice label=""d"">" + stdElementsOpen + @"choice 4" + stdElementsClose + @"</choice>" +
			@"<choice label=""e"">" + stdElementsOpen + @"choice 5" + stdElementsClose + @"</choice>" +
			@"<choice label=""f"">" + stdElementsOpen + @"choice 6" + stdElementsClose + @"</choice>" +
			@"<choice label=""g"">" + stdElementsOpen + @"choice 7" + stdElementsClose + @"</choice>" +
			@"<choice label=""h"">" + stdElementsOpen + @"choice 8" + stdElementsClose + @"</choice>" +
			@"<choice label=""i"">" + stdElementsOpen + @"choice 9" + stdElementsClose + @"</choice>" +
			@"<choice label=""j"">" + stdElementsOpen + @"choice 10" + stdElementsClose + @"</choice>" +
			@"<choice label=""k"">" + stdElementsOpen + @"choice 11" + stdElementsClose + @"</choice>" +
			@"<choice label=""l"">" + stdElementsOpen + @"choice 12" + stdElementsClose + @"</choice>" +
			@"<choice label=""m"">" + stdElementsOpen + @"choice 13" + stdElementsClose + @"</choice>" +
			@"<choice label=""n"">" + stdElementsOpen + @"choice 14" + stdElementsClose + @"</choice>" +
			@"<choice label=""o"">" + stdElementsOpen + @"choice 15" + stdElementsClose + @"</choice>" +
			@"<choice label=""p"">" + stdElementsOpen + @"choice 16" + stdElementsClose + @"</choice>" +
			@"<choice label=""q"">" + stdElementsOpen + @"choice 17" + stdElementsClose + @"</choice>" +
			@"<choice label=""r"">" + stdElementsOpen + @"choice 18" + stdElementsClose + @"</choice>" +
			@"<choice label=""s"">" + stdElementsOpen + @"choice 19" + stdElementsClose + @"</choice>" +
			@"<choice label=""t"">" + stdElementsOpen + @"choice 20" + stdElementsClose + @"</choice>" +
			@"<choice label=""u"">" + stdElementsOpen + @"choice 21" + stdElementsClose + @"</choice>" +
			@"<choice label=""v"">" + stdElementsOpen + @"choice 22" + stdElementsClose + @"</choice>" +
			@"<choice label=""w"">" + stdElementsOpen + @"choice 23" + stdElementsClose + @"</choice>" +
			@"<choice label=""x"">" + stdElementsOpen + @"choice 24" + stdElementsClose + @"</choice>" +
			@"<choice label=""y"">" + stdElementsOpen + @"choice 25" + stdElementsClose + @"</choice>" +
			@"<choice label=""z"">" + stdElementsOpen + @"choice 26" + stdElementsClose + @"</choice>" +
			@"<choice label=""aa"">" + stdElementsOpen + @"choice 27" + stdElementsClose + @"</choice>" +
			@"</mc>" + Environment.NewLine;

		private static readonly string mcqRtfString =
			@"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
			@"{\fonttbl" + Environment.NewLine + @"{\f0\fswiss Arial;}" + Environment.NewLine + @"}" + Environment.NewLine +
			@"\fs20{\colortbl;" + Environment.NewLine +
			@"\red0\green0\blue0;" + Environment.NewLine + @"\red255\green255\blue255;" + Environment.NewLine + @"}" + Environment.NewLine +
			@"\deftab0\tx2880\pard \tx2880{\f0\fs20\cf1 Choose:}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    a) }{\f0\fs20\cf1 choice 1}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    b) }{\f0\fs20\cf1 choice 2}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    c) }{\f0\fs20\cf1 choice 3}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    d) }{\f0\fs20\cf1 choice 4}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    e) }{\f0\fs20\cf1 choice 5}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    f) }{\f0\fs20\cf1 choice 6}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    g) }{\f0\fs20\cf1 choice 7}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    h) }{\f0\fs20\cf1 choice 8}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    i) }{\f0\fs20\cf1 choice 9}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    j) }{\f0\fs20\cf1 choice 10}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    k) }{\f0\fs20\cf1 choice 11}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    l) }{\f0\fs20\cf1 choice 12}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    m) }{\f0\fs20\cf1 choice 13}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    n) }{\f0\fs20\cf1 choice 14}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    o) }{\f0\fs20\cf1 choice 15}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    p) }{\f0\fs20\cf1 choice 16}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    q) }{\f0\fs20\cf1 choice 17}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    r) }{\f0\fs20\cf1 choice 18}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    s) }{\f0\fs20\cf1 choice 19}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    t) }{\f0\fs20\cf1 choice 20}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    u) }{\f0\fs20\cf1 choice 21}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    v) }{\f0\fs20\cf1 choice 22}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    w) }{\f0\fs20\cf1 choice 23}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    x) }{\f0\fs20\cf1 choice 24}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    y) }{\f0\fs20\cf1 choice 25}" +
			@"\par \pard \tx2880{\f0\fs20\cf1    z) }{\f0\fs20\cf1 choice 26}" +
			@"\par \pard \tx2880{\f0\fs20\cf1   aa) }{\f0\fs20\cf1 choice 27}" +
			@"\par }";

		[Test]
		public void McqWith27ChoicesFromRtfGeneratesProperXml()
		{
			var mcItem = new McqItem();
			mcItem.Rtf = mcqRtfString;

			Assert.AreEqual(mcqXmlString, mcItem.ToXml("Q1"));
		}

		[Test]
		public void McqWith27ChoicesFromXmlGeneratesProperRtf()
		{
			Util.NewTestProject();
			var form = Project.Current.AddForm();

			var mcItem = new McqItem(new XmlElement(mcqXmlString));
			form.ItemList.Add(mcItem);

			Assert.AreEqual(mcqRtfString, mcItem.ToRtf());
		}
	}
}