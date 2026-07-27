// $Workfile: FormPreviewTest.cs $
// $Revision: 9 $	$Date: 10/29/07 11:02a $
// Copyright © 2005 Douglas G. Carlston. All rights reserved.

using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Common;
using System.Xml.XPath;
using System.IO;
using Tawala.Projects.Forms;

namespace TawalaTest.FunctionalityTest
{
	[TestFixture]
	public class FormPreviewTest
	{
		private string xmlRequestStringTemplate = "<request type=\"previewForm\" form=\"{0}\" protocol=\"1.0\">";
		private string xmlRequestString;
		private string expectedUrl;

		private IForm form1;
		private IForm form2;

		[SetUp]
		public void SetUp()
		{
			Project.NewTestProject();
			Project.Current.Name = "Form Preview Test";

			form1 = Project.Current.AddForm();
			form2 = Project.Current.AddForm();

			form1.ItemList.Add(new TextItem());
			form1.ItemList.Add(new FibItem());
			form1.ItemList.Add(new McqItem());

			form2.ItemList.Add(new TextItem());
			form2.ItemList.Add(new FibItem());
			form2.ItemList.Add(new McqItem());
		}

		[Test]
		public void PreviewForm1()
		{
			xmlRequestString = string.Format(xmlRequestStringTemplate, form1.Name);

			expectedUrl = "http://build.tawala.com/f/devTestUser-Form+Preview+Test/Form+1";
		}

		[Test]
		public void PreviewForm2()
		{
			xmlRequestString = string.Format(xmlRequestStringTemplate, form2.Name);

			expectedUrl = "http://build.tawala.com/f/devTestUser-Form+Preview+Test/Form+2";
		}

		[TearDown]
		public void TearDown()
		{
			XMLTransceiver transceiver = new XMLTransceiver("http://build.tawala.com/client");

			string sendString =
				"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				xmlRequestString +
				Tawala.Common.GlobalSettings.CredentialsElement("devTestUser", "JFSBKM185") +
				Project.Current.ToXml() +
				"</request>";
			Console.WriteLine(sendString);

			transceiver.Transmit(sendString);

			string rcvString = transceiver.Receive();
			Console.WriteLine(rcvString);

			XPathDocument xml = new XPathDocument(new StringReader(rcvString));
			XPathNavigator nav = xml.CreateNavigator();
			XPathNavigator status = nav.SelectSingleNode("/response/@status");
			Assert.IsNotNull(status, "Transceiver did not return a <status> tag.");

			if (status != null)
			{
				if (status.Value.CompareTo("success") == 0)
				{
					XPathNavigator id = nav.SelectSingleNode("/response/formPreview/@url");
					Assert.IsNotNull(id, "Transceiver did not return a <formPreview> tag with an \"url\" attribute.");

					if (id != null)
					{
						Assert.AreEqual(expectedUrl, id.Value);
					}
				}
				else
				{
					Assert.Fail("Transceiver returned non-success status.");
				}
			}
		}
	}
}
