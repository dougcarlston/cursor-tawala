using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;
using NUnit.Framework;
using TawalaTest.ProjectTest;

namespace ProjectConverterTest
{
	[TestFixture]
	public class XmlReadTest : TestBase
	{
		XmlTextReader reader;


		[SetUp]
		public void Setup()
		{
			reader = new XmlTextReader(GetTestFilePath("TestFile.xml"));

		}

		[TearDown]
		public void TearDown()
		{
		}

		/// <summary>
		/// Verify existence of XML file for testing
		/// </summary>
		[Test]
		public void CheckForTestFile()
		{
			Assert.IsTrue(File.Exists(TawalaTest.TestSupport.Util.GetTestFilePath("TestFile.xml")), "Testfile.xml does not exist");

			try
			{
				reader.Read();
			}
			catch
			{
				Assert.Fail("TestFile.xml is not a valid XML file");
			}
		}

		/// <summary>
		/// Indicates whether the XML file contains the specified element.
		/// </summary>
		public bool HasElement(string elementName)
		{
			bool hasElement = false;

			try
			{
				hasElement = reader.ReadToFollowing(elementName);
			}
			catch
			{
				Assert.Fail("Error processing TestFile.xml");
			}

			return hasElement;
		}


		/// <summary>
		/// Reads the specified attribute from the specified element
		/// </summary>
		public string GetAttributeValue(string elementName, string attributeName)
		{
			string attributeValue = "";

			try
			{
				// move reader to specified element
				reader.ReadToFollowing(elementName);


				// while there's an attribute to read...
				while (reader.MoveToNextAttribute())
				{
					if (reader.Name == attributeName)
					{
						attributeValue = reader.Value;

						return attributeValue;
					}
				}

			}
			catch
			{
				Assert.Fail("Error processing TestFile.xml");
			}

			return attributeValue;
		}


		/// <summary>
		/// Reads text from the specified element
		/// </summary>
		public string GetTextValue(string elementName)
		{
			string textValue = "";

			try
			{
				// move reader to specified element
				reader.ReadToFollowing(elementName);

				// get text from element
				textValue = reader.ReadElementContentAsString();

				return textValue;

			}
			catch
			{
				Assert.Fail("Error processing TestFile.xml");
			}

			return textValue;
		}


		/// <summary>
		/// Verify that project element can be read from XML file
		/// </summary>
		[Test]
		public void ReadProjectElement()
		{
			bool hasElement = HasElement("project");

			Assert.IsTrue(hasElement, "Project element not found");
		}


		/// <summary>
		/// Verify that project name attribute can be read from XML file
		/// </summary>
		[Test]
		public void ReadProjectNameAttribute()
		{
			string nameValue = GetAttributeValue("project", "name");

			Assert.AreEqual("MyProject", nameValue);
		}


		/// <summary>
		/// Verify that project designer attribute can be read from XML file
		/// </summary>
		[Test]
		public void ReadProjectDesignerAttribute()
		{
			string designerValue = GetAttributeValue("project", "designer");

			Assert.AreEqual("Guest", designerValue);
		}


		/// <summary>
		/// Verify that project format attribute can be read from XML file
		/// </summary>
		[Test]
		public void ReadProjectFormatAttribute()
		{
			string formatValue = GetAttributeValue("project", "format");

			Assert.AreEqual("1.1", formatValue);
		}


		/// <summary>
		/// Verify that forms element can be read from XML file
		/// </summary>
		[Test]
		public void ReadFormsElement()
		{
			bool hasElement = HasElement("forms");

			Assert.IsTrue(hasElement, "forms element not found");
		}


		/// <summary>
		/// Verify that form element can be read from XML file
		/// </summary>
		[Test]
		public void ReadFormElement()
		{
			bool hasElement = HasElement("form");

			Assert.IsTrue(hasElement, "form element not found");
		}


		/// <summary>
		/// Verify that form name attribute can be read from XML file
		/// </summary>
		[Test]
		public void ReadFormNameAttribute()
		{
			string nameValue = GetAttributeValue("form", "name");

			Assert.AreEqual("Form 1", nameValue);
		}


		/// <summary>
		/// Verify that fib element can be read from XML file
		/// </summary>
		[Test]
		public void ReadFibElement()
		{
			bool hasElement = HasElement("fib");

			Assert.IsTrue(hasElement, "fib element not found");
		}


		/// <summary>
		/// Verify that children of fib element can be read from XML file
		/// </summary>
		[Test]
		public void ReadFibElementChildren()
		{
			int elementNodeCount = 0;
			int textNodeCount = 0;

			try
			{
				// find the first fib node
				reader.ReadToFollowing("fib");

				// create new reader to read descendant nodes
				XmlReader childReader = reader.ReadSubtree();

				// consume the fib node
				childReader.Read();

				// while a descendant node exists...
				while (childReader.Read())
				{
					switch (childReader.NodeType)
					{
						case XmlNodeType.Element:
							elementNodeCount++;
							break;
							
						case XmlNodeType.Text:
							textNodeCount++;
							break;
					}
				}

				childReader.Close();

			}
			catch
			{
				Assert.Fail("Error processing TestFile.xml");
			}

			Assert.AreEqual(1, elementNodeCount, "Element node count is incorrect");
			Assert.AreEqual(1, textNodeCount, "Text node count is incorrect");

		}


		/// <summary>
		/// Verify that items element can be read from XML file
		/// </summary>
		[Test]
		public void ReadItemsElement()
		{
			bool hasElement = HasElement("items");

			Assert.IsTrue(hasElement, "items element not found");
		}


		/// <summary>
		/// Verify that text element can be read from XML file
		/// </summary>
		[Test]
		public void ReadTextElement()
		{
			bool hasElement = HasElement("text");

			Assert.IsTrue(hasElement, "text element not found");
		}


		/// <summary>
		/// Verify that text label attribute can be read from XML file
		/// </summary>
		[Test]
		public void ReadTextLabelAttribute()
		{
			string nameValue = GetAttributeValue("text", "label");

			Assert.AreEqual("T1", nameValue);
		}


		/// <summary>
		/// Verify that text element contents can be read from XML file
		/// </summary>
		[Test]
		public void ReadTextContents()
		{
			string textValue = GetTextValue("text");

			Assert.AreEqual("This is a text item in Form 1.", textValue);
		}

		/// <summary>
		/// Verify that document element can be read from XML file
		/// </summary>
		[Test]
		public void ReadDocumentElement()
		{
			bool hasElement = HasElement("document");

			Assert.IsTrue(hasElement, "document element not found");
		}


		/// <summary>
		/// Verify that document name attribute can be read from XML file
		/// </summary>
		[Test]
		public void ReadDocumentNameAttribute()
		{
			string nameValue = GetAttributeValue("document", "name");

			Assert.AreEqual("Document 1", nameValue);
		}


		/// <summary>
		/// Verify that children of document element can be read from XML file
		/// </summary>
		[Test]
		public void ReadDocumentElementChildren()
		{
			int elementNodeCount = 0;
			int textNodeCount = 0;

			try
			{
				// find the first document node
				reader.ReadToFollowing("document");

				// create new reader to read descendant nodes
				XmlReader childReader = reader.ReadSubtree();

				// consume the document node
				childReader.Read();

				// while a descendant node exists...
				while (childReader.Read())
				{
					switch (childReader.NodeType)
					{
						case XmlNodeType.Element:
							elementNodeCount++;
							break;

						case XmlNodeType.Text:
							textNodeCount++;
							break;
					}
				}

				childReader.Close();

			}
			catch
			{
				Assert.Fail("Error processing TestFile.xml");
			}

			Assert.AreEqual(1, elementNodeCount, "Element node count is incorrect");
			Assert.AreEqual(1, textNodeCount, "Text node count is incorrect");

		}


		/// <summary>
		/// Verify that show element can be read from XML file
		/// </summary>
		[Test]
		public void ReadShowElement()
		{
			bool hasElement = HasElement("show");

			Assert.IsTrue(hasElement, "show element not found");
		}


		/// <summary>
		/// Verify that show document attribute can be read from XML file
		/// </summary>
		[Test]
		public void ReadShowDocumentAttribute()
		{
			string nameValue = GetAttributeValue("show", "document");

			Assert.AreEqual("Document 1", nameValue);
		}


		/// <summary>
		/// Verify that if element can be read from XML file
		/// </summary>
		[Test]
		public void ReadIfElement()
		{
			bool hasElement = HasElement("if");

			Assert.IsTrue(hasElement, "if element not found");
		}


		/// <summary>
		/// Verify that if statement conditions can be read from XML file
		/// </summary>
		[Test]
		public void ReadIfConditions()
		{
			int conditionsCount = 0;
			int isNotBlankCount = 0;

			try
			{
				// find the first if node
				reader.ReadToFollowing("if");

				// create new reader to read descendant nodes
				XmlReader ifReader = reader.ReadSubtree();

				// consume the if node
				ifReader.Read();

				// while a descendant node exists...
				while (ifReader.Read())
				{
					switch (ifReader.Name)
					{
						case "conditions":
							conditionsCount++;

							// create new reader to read descendant nodes
							XmlReader conditionReader = ifReader.ReadSubtree();

							// consume the conditions node
							conditionReader.Read();

							// while a descendant node exists...
							while (conditionReader.Read())
							{
								switch (conditionReader.Name)
								{
									case "isNotBlank":
										isNotBlankCount++;
										break;
								}
							}

							break;
					}
				}

				ifReader.Close();

			}
			catch
			{
				Assert.Fail("Error processing TestFile.xml");
			}

			Assert.AreEqual(1, conditionsCount, "Conditions count is incorrect");
			Assert.AreEqual(1, isNotBlankCount, "isNotBlank count is incorrect");
		}


		/// <summary>
		/// Verify that isNotBlank element can be read from XML file
		/// </summary>
		[Test]
		public void ReadIsNotBlankElement()
		{
			bool hasElement = HasElement("isNotBlank");

			Assert.IsTrue(hasElement, "isNotBlank element not found");
		}


	}
}
