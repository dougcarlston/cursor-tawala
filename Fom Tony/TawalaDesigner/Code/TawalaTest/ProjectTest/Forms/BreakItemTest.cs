using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the TestFormItem class
	/// </summary>
	[TestFixture]
	public class BreakItemTest
	{
		[Test]
		public void NewBreakItem() 
		{ 
			BreakItem item = new BreakItem();

			//Assertions 
			Assert.AreEqual(false, item.IsTextItem);
			Assert.AreEqual(false, item.IsQuestionItem);
		} 

		[Test]
		public void GetXml() 
		{ 
			BreakItem item = new BreakItem();

			//Assertions 
			Assert.AreEqual("<break/>\r\n", item.ToXml());
		}

		[Test]
		public void AlternateLabel()
		{
			BreakItem item = new BreakItem();

			//Assertions 
			Assert.AreEqual(string.Empty, item.AlternateLabel);

			// assignment should have no effect
			item.AlternateLabel = "Foo";

			Assert.AreEqual(string.Empty, item.AlternateLabel);
		}
	}
}
