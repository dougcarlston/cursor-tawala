using System;
using System.IO;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class ForwardFieldReferenceInTableCell712
	{
		[SetUp]
		public void Setup()
		{
			Util.NewTestProject();
		}

		[Test]
		public void CanConstructFormFromXmlWithForwardFieldReferenceInTextItem()
		{
			string xmlHeaderString =
				@"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine;

			string xmlBodyString =
				@"<project name=""AAA Test"" themePath=""default"" format=""" + Project.XmlFormatVersion + @""" designerBuild=""0"">" + Environment.NewLine +
				@"<pageHeader></pageHeader>" +
				@"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" + Environment.NewLine +
				@"<items>" + Environment.NewLine +
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<tabPositions>" +
				@"<tabStop position=""2880""/>" +
				@"</tabPositions>" +
				@"<font>" +
				@"<field name=""Form 1:Field1""/>" +
				@"</font>" +
				@"</paragraph>" +
				@"</text>" + Environment.NewLine +
				@"<field name=""Field1""/>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>" + Environment.NewLine +
				@"</forms>" + Environment.NewLine +
				@"</project>" + Environment.NewLine;

			string projectXmlString = xmlHeaderString + xmlBodyString;

			using (MemoryStream xmlStream = new MemoryStream())
			{
				byte[] xmlByteArray = System.Text.Encoding.UTF8.GetBytes(projectXmlString);
				xmlStream.Write(xmlByteArray, 0, xmlByteArray.Length);

				TawalaProjectConverter converter = new TawalaProjectConverter(xmlStream);
				converter.ConvertXmlToProject();
			}

			Assert.AreEqual(xmlBodyString, Project.Current.ToXml());
		}

		[Test]
		public void CanConstructFormFromXmlWithForwardFieldReferenceInTableCell()
		{
			string xmlHeaderString =
				@"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine;

			string xmlBodyString =
				@"<project name=""AAA Test"" themePath=""default"" format=""" + Project.XmlFormatVersion + @""" designerBuild=""0"">" + Environment.NewLine +
				@"<pageHeader></pageHeader>" +
				@"<forms>" + Environment.NewLine +
                @"<form name=""Form 1"" startPoint=""true"" themePath=""default"" blockBackButton=""false"">" + Environment.NewLine +
				@"<items>" + Environment.NewLine +
				@"<text label=""T1"" style=""normal"">" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<tabPositions>" +
				@"<tabStop position=""2880""/>" +
				@"</tabPositions>" +
				@"</paragraph>" +
				@"<table indent=""0"">" +
				@"<row>" +
				@"<cell width=""5400"">" +
				@"<division indent=""0"" align=""left"">" +
				@"<font>" +
				@"<field name=""Form 1:Field1""/>" +
				@"</font>" +
				@"</division>" +
				@"</cell>" +
				@"</row>" +
				@"</table>" +
				@"</text>" + Environment.NewLine +
				@"<field name=""Field1""/>" + Environment.NewLine +
				@"</items>" + Environment.NewLine +
				@"</form>" + Environment.NewLine +
				@"</forms>" + Environment.NewLine +
				@"</project>" + Environment.NewLine;

			string projectXmlString = xmlHeaderString + xmlBodyString;

			using (MemoryStream xmlStream = new MemoryStream())
			{
				byte[] xmlByteArray = System.Text.Encoding.UTF8.GetBytes(projectXmlString);
				xmlStream.Write(xmlByteArray, 0, xmlByteArray.Length);

				TawalaProjectConverter converter = new TawalaProjectConverter(xmlStream);
				converter.ConvertXmlToProject();
			}

			Assert.AreEqual(xmlBodyString, Project.Current.ToXml());
		}

	}
}