using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Summary description for FormItemTest.
	/// </summary>
	[TestFixture]
	public class FormItemTest
	{
		[SetUp]
		public void MethodSetup()
		{
			TestSupport.Util.NewTestProject();
		}

		[Test]
		public void NewFormItem() 
		{ 
			FormItem item = new FormItem();

			//Assertions 
			Assert.AreEqual(string.Empty, item.AlternateLabel);
		} 

		[Test]
		public void FormItemSetText() 
		{ 
			FormItem item = new FormItem();

			item.Text = "Here's some text for content.";

			//Assertions 
			Assert.AreEqual("Here's some text for content.", item.Text);
		} 

		[Test]
		public void IsTextItem() 
		{ 
			FormItem item = new FormItem();

			//Assertions 
			Assert.AreEqual(false, item.IsTextItem);
		} 

		[Test]
		public void IsQuestionItem() 
		{ 
			FormItem item = new FormItem();

			Assert.AreEqual(false, item.IsQuestionItem);
		}

		[Test]
		public void Id()
		{
			int nextID = Reflect<Project>.GetStaticField<int>("nextUniqueID");

			FormItem item1 = new FormItem();
			FormItem item2 = new FormItem();

			// Note:  Project.privateInvitationInviteeID = ID #1
			Assert.AreEqual(nextID, item1.Id);
			Assert.AreEqual(nextID+1, item2.Id);
		}

		[Test]
		public void AlternateLabel()
		{
			FormItem item = new FormItem();

			Assert.AreEqual(string.Empty, item.AlternateLabel);

			item.AlternateLabel = "Foo";

			Assert.AreEqual("Foo", item.AlternateLabel);

			item.AlternateLabel = null;

			Assert.AreEqual(string.Empty, item.AlternateLabel);
		}

		[Test]
		public void AlternateLabelWithWhitespace()
		{
			FormItem item = new FormItem();
			Assert.AreEqual(string.Empty, item.AlternateLabel);

			item.AlternateLabel = "   Foo   ";
			Assert.AreEqual("Foo", item.AlternateLabel);
		}

		[Test]
		public void GetAlternateLabelXml()
		{
			FormItem item = new FormItem();
			item.AlternateLabel = "BarCode";
			Assert.AreEqual(" alternateLabel=\"BarCode\"", item.GetAlternateLabelXml());
		}
	}
}
