using System.IO;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

using NUnit.Framework;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class SpecialXmlCharactersInAlternateLabelCauseBadSave501
    {
		[SetUp]
		public void Setup()
		{
			TestSupport.Util.NewTestProject();

			IForm form1 = Project.Current.AddForm();
			FibItem fib = new FibItem();
			form1.ItemList.Add(fib);
			Blank blank = fib.BlankList[0];
			blank.Length = 1;
			blank.AlternateLabel = testLabel;
		}

		private const string testLabel = "\"<This & That>\"";

		[Test]
		public void SpecialXmlCharsInBlankAlternateLabelDoNotCauseBadSave()
		{
			Util.SaveAndReloadCurrentProject();

			IForm form1 = Project.Current.FormList[0];
			FibItem fib = form1.ItemList[0] as FibItem;
			Blank blank = fib.BlankList[0] as Blank;

			Assert.AreEqual(blank.Length, 1);
			Assert.AreEqual(testLabel, blank.AlternateLabel);
		}
	}
}
