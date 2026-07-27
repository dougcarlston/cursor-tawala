using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using System.Reflection;
using Tawala.Projects.Fields;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class FieldUtilTest
	{
		[Test]
		public void GetRecordNameFromRecordAndFormQualifiedMCQ()
		{
			Assert.AreEqual("Record", FieldUtil.GetRecordName("Record:Form 1:Q1"));
		}

		[Test]
		public void GetRecordNameFromRecordAndFormQualifiedBlank()
		{
			Assert.AreEqual("Record", FieldUtil.GetRecordName("Record:Form 1:Q1:a"));
			Assert.AreEqual("", FieldUtil.GetRecordName("Form 1:Q1:a"));
		}

		[Test]
		public void GetFormNameFromRecordAndFormQualifiedMCQ()
		{
			Assert.AreEqual("Form 1", FieldUtil.GetFormName("Record:Form 1:Q1"));
			Assert.AreEqual("", FieldUtil.GetRecordName("Q1:a"));
		}

		[Test]
		public void GetFormNameFromRecordAndFormQualifiedBlank()
		{
			Assert.AreEqual("Form 1", FieldUtil.GetFormName("Record:Form 1:Q1:a"));
		}

		[Test]
		public void GetFormNameFromFormQualifiedMCQ()
		{
			Assert.AreEqual("Form 1", FieldUtil.GetFormName("Form 1:Q1"));
		}

		[Test]
		public void GetFormNameFromFormQualifiedBlank()
		{
			Assert.AreEqual("Form 1", FieldUtil.GetFormName("Form 1:Q1:a"));
		}

		[Test]
		public void GetFieldNameFromRecordAndFormQualifiedMCQ()
		{
			Assert.AreEqual("Q1", FieldUtil.GetFieldName("Record:Form 1:Q1"));
		}

		[Test]
		public void GetFieldNameFromRecordAndFormQualifiedBlank()
		{
			Assert.AreEqual("Q1:a", FieldUtil.GetFieldName("Record:Form 1:Q1:a"));
		}

		[Test]
		public void GetFieldNameFromFormQualifiedMCQ()
		{
			Assert.AreEqual("Q1", FieldUtil.GetFieldName("Form 1:Q1"));
		}

		[Test]
		public void GetFieldNameFromFormQualifiedBlank()
		{
			Assert.AreEqual("Q1:a", FieldUtil.GetFieldName("Form 1:Q1:a"));
		}

		[Test]
		public void GetFieldNameFromUnqualifiedMCQ()
		{
			Assert.AreEqual("Q1", FieldUtil.GetFieldName("Q1"));
		}

		[Test]
		public void GetFieldNameFromUnqualifiedBlank()
		{
			Assert.AreEqual("Q1:a", FieldUtil.GetFieldName("Q1:a"));
		}

		// JDF NEW STUFF:
		private static string sharedDataSources =
			@"<datasources>" + Environment.NewLine +
			@"<datasource name=""DSN1"">" + Environment.NewLine +
			@"<field name=""Q1:a"" type=""string"" />" + Environment.NewLine +
			@"<field name=""Q2"" type=""mcq"" choices=""1"" onlyone=""false"" />" + Environment.NewLine +
			@"</datasource>" + Environment.NewLine +
			@"</datasources>" + Environment.NewLine;

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();
			Project.Current.AddForm();
			typeof(FieldProviders).InvokeMember("initialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, null, new object[] { new Tawala.XmlSupport.XmlElement(sharedDataSources) });
		}

		[Test]
		public void _IsVariable()
		{
			Assert.AreEqual(true, FieldUtil.IsVariable("Score"));

			Assert.AreEqual(false, FieldUtil.IsVariable("Form 1:Q1:a"));
			Assert.AreEqual(false, FieldUtil.IsVariable("Form 1:Q2"));
			Assert.AreEqual(false, FieldUtil.IsVariable("Form 1:Score"));
			Assert.AreEqual(false, FieldUtil.IsVariable("Record1:Form 1:Q1:a"));
			Assert.AreEqual(false, FieldUtil.IsVariable("Record1:Form 1:Q2"));
			Assert.AreEqual(false, FieldUtil.IsVariable("Record1:Form 1:Score"));
		}

		[Test]
		public void _IsFormField()
		{
			Assert.AreEqual(true, FieldUtil.IsFormField("Form 1:Q1:a"));
			Assert.AreEqual(true, FieldUtil.IsFormField("Form 1:Q1:ab"));
			Assert.AreEqual(true, FieldUtil.IsFormField("Form 1:Q2"));
			Assert.AreEqual(true, FieldUtil.IsFormField("Form 1:FibLabel:a"));
			Assert.AreEqual(true, FieldUtil.IsFormField("Form 1:McqLabel"));

			Assert.AreEqual(false, FieldUtil.IsFormField("Form 2:Q1:a"));
			Assert.AreEqual(false, FieldUtil.IsFormField("Form 1:Q1:a$"));
			Assert.AreEqual(false, FieldUtil.IsFormField("Q1:a"));
			Assert.AreEqual(false, FieldUtil.IsFormField("FibLabel"));
			Assert.AreEqual(false, FieldUtil.IsFormField("Record1:Form 1:Q1:a"));
			Assert.AreEqual(false, FieldUtil.IsFormField("Record1:Form 1:FibLabel:a"));
		}

		[Test]
		public void _IsRecordField()
		{
			Assert.AreEqual(true, FieldUtil.IsRecordField("Record:Form 1:Q1:a"));
			Assert.AreEqual(true, FieldUtil.IsRecordField("Record:Form 1:Q2"));
			Assert.AreEqual(true, FieldUtil.IsRecordField("Record:Form 1:FibLabel:a"));
			Assert.AreEqual(true, FieldUtil.IsRecordField("Record:Form 1:McqLabel"));

			Assert.AreEqual(false, FieldUtil.IsRecordField("Record:Form 2:Q1:a"));
			Assert.AreEqual(false, FieldUtil.IsRecordField("Q1:a"));
			Assert.AreEqual(false, FieldUtil.IsRecordField("Q2"));
			Assert.AreEqual(false, FieldUtil.IsRecordField("Form 1:Q1:a"));
			Assert.AreEqual(false, FieldUtil.IsRecordField("Form 1:Q2"));
			Assert.AreEqual(false, FieldUtil.IsRecordField("Form 1:FibLabel:a"));
			Assert.AreEqual(false, FieldUtil.IsRecordField("Form 1:McqLabel"));

			Assert.IsFalse(FieldUtil.IsRecordField("Record:DSN1:Q1:a"));
			Assert.IsFalse(FieldUtil.IsRecordField("Record:DSN1:Q2"));
		}

		[Test]
		public void _IsRegularOrExternalRecordField()
		{
			Assert.IsTrue(FieldUtil.IsRegularOrExternalRecordField("Record:DSN1:Q1:a"));
			Assert.IsTrue(FieldUtil.IsRegularOrExternalRecordField("Record:DSN1:Q2"));

			Assert.IsTrue(FieldUtil.IsRegularOrExternalRecordField("Record:Form 1:Q1:a"));
			Assert.IsTrue(FieldUtil.IsRegularOrExternalRecordField("Record:Form 1:Q2"));
			Assert.IsTrue(FieldUtil.IsRegularOrExternalRecordField("Record:Form 1:FibLabel:a"));
			Assert.IsTrue(FieldUtil.IsRegularOrExternalRecordField("Record:Form 1:McqLabel"));

			Assert.IsFalse(FieldUtil.IsRegularOrExternalRecordField("Record:Form 2:Q1:a"));
			Assert.IsFalse(FieldUtil.IsRegularOrExternalRecordField("Q1:a"));
			Assert.IsFalse(FieldUtil.IsRegularOrExternalRecordField("Q2"));
			Assert.IsFalse(FieldUtil.IsRegularOrExternalRecordField("Form 1:Q1:a"));
			Assert.IsFalse(FieldUtil.IsRegularOrExternalRecordField("Form 1:Q2"));
			Assert.IsFalse(FieldUtil.IsRegularOrExternalRecordField("Form 1:FibLabel:a"));
			Assert.IsFalse(FieldUtil.IsRegularOrExternalRecordField("Form 1:McqLabel"));
		}
	}
}
