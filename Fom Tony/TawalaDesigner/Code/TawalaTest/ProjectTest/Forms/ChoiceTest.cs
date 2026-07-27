using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for the Choice class
	/// </summary>
	[TestFixture]
	public class ChoiceTest
	{
		[Test]
		public void NewChoice() 
		{ 
			Choice choice = new Choice();

			//Assertions 
			Assert.IsNotNull(choice);
			Assert.AreEqual("", choice.Text);
		} 

		[Test]
		public void NewChoiceWithText() 
		{ 
			Choice choice = new Choice("This choice has a name");

			//Assertions 
			Assert.AreEqual("This choice has a name", choice.Text);
		}

		[Test]
		public void ConstructFromXml()
		{
			string xmlString =
				"<choice label=\"a\">Choice One</choice>";

			IXmlElement element = new XmlElement(xmlString);
			Choice choice = new Choice(element);

			Assert.AreEqual("Choice One", choice.Text);
		}

		[Test]
		public void GetXml() 
		{ 
			Choice choice = new Choice();
			choice.Text = "Choice Numero Uno";

			string expString =
				"<choice label=\"a\">" +
				"<paragraph indent=\"0\" align=\"left\">" +
				XmlConstants.FullBeginFont +
				"Choice Numero Uno" +
				XmlConstants.EndFont +
				"</paragraph>" +
				"</choice>";

			Assert.AreEqual(expString, choice.ToXml("a"));
		}
	}
}
