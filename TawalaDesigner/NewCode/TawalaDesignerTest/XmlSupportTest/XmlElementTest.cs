using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Tawala.XmlSupport;

namespace TawalaTest.XmlSupportTest
{
	[TestFixture]
	public class XmlElementTest
	{
		[Test]
		public void Construct() 
		{
			string xmlString =
				"<root>\n" +
				"</root>";

			IXmlElement element = new XmlElement(xmlString);

			Assert.IsNotNull(element);
		}

		[Test]
		public void SingleEmptyChildElement()
		{
			string xmlString =
				"<root>\n" +
				"    <child/>\n" +
				"</root>";

			IXmlElement element = new XmlElement(xmlString);

			Assert.AreEqual("child", element.GetChild("child").Name);
		}

		[Test]
		public void HasChild()
		{
			string xmlString =
				"<root>\n" +
				"    <child/>\n" +
				"</root>";

			IXmlElement element = new XmlElement(xmlString);

			Assert.IsTrue(element.HasChild("child"));
		}

		[Test]
		public void GetText()
		{
			string xmlString =
				"<root>Contents of root</root>";

			IXmlElement element = new XmlElement(xmlString);

			Assert.AreEqual("root", element.Name);
			Assert.AreEqual("Contents of root", element.Text);
		}

		[Test]
		public void GetTextWithNewlineAtEnd()
		{
			string xmlString =
				"<root>Contents of root\n" +
				"</root>";

			IXmlElement element = new XmlElement(xmlString);

			Assert.AreEqual("Contents of root\n", element.Text);
		}

		[Test]
		public void GetChildText()
		{
			string xmlString =
				"<root>\n" +
				"    <child>Contents of child</child>\n" +
				"</root>";

			IXmlElement element = new XmlElement(xmlString);

			Assert.AreEqual("Contents of child", element.GetChild("child").Text);
		}

		[Test]
		public void MultipleChildElements()
		{
			string xmlString =
				"<root>\n" +
				"    <childOne/>\n" +
				"    <childTwo/>\n" +
				"    <childThree/>\n" +
				"</root>";

			IXmlElement element = new XmlElement(xmlString);

			Assert.AreEqual("childOne", element.GetChild("childOne").Name);
			Assert.AreEqual("childTwo", element.GetChild("childTwo").Name);
			Assert.AreEqual("childThree", element.GetChild("childThree").Name);
		}

		[Test]
		public void NestedChildElements()
		{
			string xmlString =
				"<root>\n" +
				"    <childOne>\n" +
				"        <childOneChild/>\n" +
				"    </childOne>\n" +
				"</root>";

			IXmlElement element = new XmlElement(xmlString);

			Assert.AreEqual("childOne", element.GetChild("childOne").Name);
			Assert.AreEqual("childOneChild", element.GetChild("childOne").GetChild("childOneChild").Name);
		}

		[Test]
		public void SingleAttribute()
		{
			string xmlString =
				"<root>\n" +
				"    <child attribute=\"attributeOne\"/>\n" +
				"</root>";

			IXmlElement element = new XmlElement(xmlString);

			Assert.AreEqual("attributeOne", element.GetChild("child").GetAttribute("attribute"));
		}

		[Test]
		public void MultipleAttributes()
		{
			string xmlString =
				"<root>\n" +
				"    <child attribute1=\"attributeOne\" attribute2=\"attributeTwo\"/>\n" +
				"</root>";

			IXmlElement element = new XmlElement(xmlString);

			Assert.AreEqual("attributeOne", element.GetChild("child").GetAttribute("attribute1"));
			Assert.AreEqual("attributeTwo", element.GetChild("child").GetAttribute("attribute2"));
		}

		[Test]
		public void IdenticallyNamedChildElements()
		{
			string xmlString =
				"<add>" +
				"  <operand field=\"Q1:a\"/>" +
				"  <operand value=\"1\"/>" +
				"</add>";

			IXmlElement element = new XmlElement(xmlString);

			Assert.AreEqual("operand", element.GetChild(0).Name);
			Assert.AreEqual("Q1:a", element.GetChild(0).GetAttribute("field"));
			Assert.AreEqual("operand", element.GetChild(1).Name);
			Assert.AreEqual("1", element.GetChild(1).GetAttribute("value"));
		}


		[Test]
		public void ElementWithCDATA()
		{
			string xmlString =
				"<htmlData>" +
				"<![CDATA[" +
				"Something inside the CDATA block" +
				"]]>" +
				"</htmlData>";

			IXmlElement element = new XmlElement(xmlString);

			Assert.AreEqual("Something inside the CDATA block", element.GetChild(0).Value);
		}

		[Test]
		public void GetChildren()
		{
			string xmlString =
				"<root>\n" +
				"    <child/>\n" +
				"    <child/>\n" +
				"    <child/>\n" +
				"</root>";

			IXmlElement element = new XmlElement(xmlString);

			Assert.AreEqual(3, element.GetChildren("child").Count);
		}

		[Test]
		public void GetDescendants()
		{
			string xmlString =
				"<root>\n" +
				"    <child>\n" +
				"    <child/>\n" +
				"        <target name=\"descendant1\"/>\n" +
				"    </child>\n" +
				"    <child>\n" +
				"        <target name=\"descendant2\"/>\n" +
				"        <grandchild>\n" +
				"            <target name=\"descendant3\"/>\n" +
				"        </grandchild>\n" +
				"        <target name=\"descendant4\"/>\n" +
				"    </child>\n" +
				"</root>";

			IXmlElement element = new XmlElement(xmlString);

			Collection<XmlElement> descendants = element.GetDescendants("target");

			Assert.AreEqual(4, descendants.Count);
			Assert.AreEqual("descendant1", descendants[0].GetAttribute("name"));
			Assert.AreEqual("descendant2", descendants[1].GetAttribute("name"));
			Assert.AreEqual("descendant3", descendants[2].GetAttribute("name"));
			Assert.AreEqual("descendant4", descendants[3].GetAttribute("name"));
		}

		[Test]
		public void CanGetDescendantsBasedOnMultipleNameStrings()
		{
			string xmlString =
				"<root>" +
				"<child>" +
				"<target name=\"descendant1\"/>" +
				"</child>" +
				"<child>" +
				"<target name=\"descendant2\"/>" +
				"<grandchild>" +
				"<target name=\"descendant3\"/>" +
				"</grandchild>" +
				"<target name=\"descendant4\"/>" +
				"</child>" +
				"<child>" +
				"Text" +
				"</child>" +
				"</root>";

			IXmlElement element = new XmlElement(xmlString);

			Collection<XmlElement> descendants = element.GetDescendants("target", "#text");

			Assert.AreEqual(5, descendants.Count);
			Assert.AreEqual("descendant1", descendants[0].GetAttribute("name"));
			Assert.AreEqual("descendant2", descendants[1].GetAttribute("name"));
			Assert.AreEqual("descendant3", descendants[2].GetAttribute("name"));
			Assert.AreEqual("descendant4", descendants[3].GetAttribute("name"));
			Assert.AreEqual("#text", descendants[4].Name);
		}


		[Test]
		public void TestMethodName()
		{
			string xmlString =
				"<choice label=\"a\">Choice Text<field name=\"Field\"/></choice>";

			IXmlElement element = new XmlElement(xmlString);

			Collection<XmlElement> descendants = element.GetDescendants("#text", "field");

			Assert.AreEqual(2, descendants.Count);
			Assert.AreEqual("#text", descendants[0].Name);
			Assert.AreEqual("field", descendants[1].Name);
		}
	}
}
