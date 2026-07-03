using System;
using NUnit.Framework;
using Tawala.Common;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the BlankList class
	/// </summary>
	[TestFixture]
	public class BlankListTest
	{
		private FibItem fibItem;
		private BlankList blankList;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			fibItem = new FibItem();
			blankList = new BlankList();
		}


		[Test]
		public void GetLabelInt()
		{
			Blank blank1 = new Blank(fibItem, 10);
			Blank blank2 = new Blank(fibItem, 20);
			blank2.AlternateLabel = "Blank Two";

			blankList.Add(blank1);
			blankList.Add(blank2);

			Assert.AreEqual("a", blankList.GetLabel(0));
			Assert.AreEqual("Blank Two", blankList.GetLabel(1));
		}

		[Test]
		public void GetLabelBlank()
		{
			Blank blank1 = new Blank(fibItem, 10);
			blank1.AlternateLabel = "Blank One";

			Blank blank2 = new Blank(fibItem, 20);
			Blank blank3 = new Blank(fibItem, 30);

			blankList.Add(blank1);
			blankList.Add(blank2);

			Assert.AreEqual("Blank One", blankList.GetLabel(blank1));
			Assert.AreEqual("b", blankList.GetLabel(blank2));
			Assert.AreEqual(null, blankList.GetLabel(blank3));
		}

	}
}
