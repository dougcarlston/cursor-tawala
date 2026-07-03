using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using TawalaTest.TestingSupport;

namespace TawalaTest.ProjectTest.Forms.NewFormItems
{
	/// <summary>
	/// Test class for the NewMcqItem class
	/// </summary>
	[TestFixture]
	public class NewMcqItemTest
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
			ComponentMaker.UseNewComponents(true);
		}

		[TearDown]
		public void TearDown()
		{
			ComponentMaker.UseNewComponents(false);
		}

		[Test]
		public void NewMcqItemIsAnIFormItem()
		{
			NewMcqItem mcqItem = new NewMcqItem();

			Assert.IsTrue(mcqItem is IFormItem);
		}

		[Test]
		public void CanGetContentsXml()
		{
			NewMcqItem fibItem = new NewMcqItem();

			string expectedXml =
				@"<question>" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">[Replace this with your question. Use Enter key to add choices below.]</font>" +
				@"</paragraph>" +
				@"</question>" +
				@"<choice label=""a"">" +
				@"</choice>";

            Assert.AreEqual(expectedXml, fibItem.Contents.ToXml());
		}

		[Test]
		public void CanGetContentsXhtml()
		{
			IForm form = Project.Current.AddForm();
			NewMcqItem mcqItem = new NewMcqItem();
			form.ItemList.Add(mcqItem);

			string expectedXhtml =
				@"<question>" +
				@"<p style=""margin-left: 0pt"" align=""left"">" +
				@"<span style=""font-family: Arial;font-size: 10pt;color: #000000;"">" +
				@"[Replace this with your question. Use Enter key to add choices below.]" +
				@"</span>" +
				@"</p>" +
				@"</question>" +
				@"<t:Choice>" +
				@"<span class=""choice"">a</span><input type=""radio"" />" +
				@"<span></span>" +
				@"<br />" +
				@"</t:Choice>";

            Assert.AreEqual(expectedXhtml, mcqItem.Contents.ToXhtml(mcqItem));
		}

		[Test]
		public void CanSetContentsFromXhtml()
		{
			IForm form = Project.Current.AddForm();
			NewMcqItem mcqItem = new NewMcqItem();
			form.ItemList.Add(mcqItem);

			string htmlString =
				@"<div>" +
				@"<question>" +
				@"<p style=""margin-left: 0pt"" align=""left"">" +
				@"<span style=""font-family: Arial;font-size: 10pt;color: #000000;"">" +
				@"[Replace this with your question. Use Enter key to add choices below.]" +
				@"</span>" +
				@"</p>" +
				@"</question>" +
				@"<t:Choice>" +
				@"<span class=""choice"">a</span><input type=""radio"">" +
				@"<span>Choice One</span>" +
				@"<br>" +
				@"</t:Choice>" +
				@"</div>";

			string expectedXml =
				@"<question>" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"[Replace this with your question. Use Enter key to add choices below.]" +
				@"</font>" +
				@"</paragraph>" +
				@"</question>" +
				@"<choice label=""a"">" +
				@"Choice One" +
				@"</choice>";

            mcqItem.Contents = FormItemContentsFactory.MakeChildrenFromHtml(htmlString);

            Assert.AreEqual(expectedXml, mcqItem.Contents.ToXml());
		}


		[Test]
		public void CanGetDefaultXml()
		{
			NewMcqItem mcqItem = new NewMcqItem();

			string expectedXml =
				@"<mc label=""Q1"" onlyone=""true"" required=""false"" style=""vertical"">" +
				@"<question>" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"[Replace this with your question. Use Enter key to add choices below.]" +
				@"</font>" +
				@"</paragraph>" +
				@"</question>" +
				@"<choice label=""a"">" +
				@"</choice>" +
				@"</mc>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, mcqItem.ToXml("Q1"));
		}

		[Test]
		public void FormItemListContainsMcqItem()
		{
			NewMcqItem mcqItem = new NewMcqItem();
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(mcqItem);

			Assert.IsTrue(Project.Current.FormList[0].ItemList.Contains(mcqItem));
		}

		[Test]
		public void FormItemListYieldsMcqItemDefaultLabel()
		{
			NewMcqItem mcqItem = new NewMcqItem();
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(mcqItem);

			Assert.AreEqual("Q1", Project.Current.FormList[0].ItemList.GetDefaultLabel(mcqItem));
		}

		[Test]
		public void McqWithAlternateLabelGeneratesExpectedMcqXml()
		{
			NewMcqItem mcqItem = new NewMcqItem();
			mcqItem.AlternateLabel = "McqAlternateLabel";

			string expectedXml =
				@"<mc label=""Q1"" alternateLabel=""McqAlternateLabel"" onlyone=""true"" required=""false"" style=""vertical"">" +
				@"<question>" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"[Replace this with your question. Use Enter key to add choices below.]" +
				@"</font>" +
				@"</paragraph>" +
				@"</question>" +
				@"<choice label=""a"">" +
				@"</choice>" +
				@"</mc>" + Environment.NewLine;

			Assert.AreEqual(expectedXml, mcqItem.ToXml("Q1"));
		}

		[Test]
		public void McqWithAlternateLabelRegeneratesCorrectlyFromXml()
		{
			string xmlString =
				@"<mc label=""Q1"" alternateLabel=""McqAlternateLabel"" onlyone=""true"" required=""false"" style=""vertical"">" +
				@"<question>" +
				@"<paragraph indent=""0"" align=""left"">" +
				@"<font face=""Arial"" size=""200"" color=""000000"">" +
				@"[Replace this with your question. Use Enter key to add choices below.]" +
				@"</font>" +
				@"</paragraph>" +
				@"</question>" +
				@"<choice label=""a"">" +
				@"<paragraph indent=""0"" align=""left""></paragraph>" +
				@"</choice>" +
				@"</mc>" + Environment.NewLine;

			IMcqItem mcqItem = new NewMcqItem(new XmlElement(xmlString));

			Assert.AreEqual("McqAlternateLabel", mcqItem.AlternateLabel);
		}
		
		private readonly string mcqXmlWithRequiredAttributeSetToTrue =
			@"<mc label=""Q1"" onlyone=""true"" required=""true"" style=""vertical"">" +
			@"<question>" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"<font face=""Arial"" size=""200"" color=""000000"">" +
			@"[Replace this with your question. Use Enter key to add choices below.]" +
			@"</font>" +
			@"</paragraph>" +
			@"</question>" +
			@"<choice label=""a"">" +
			@"</choice>" +
			@"</mc>" + Environment.NewLine;

		[Test]
		public void McqWithRequiredAttributeSetToTrueGeneratesExpectedMcqXml()
		{
			NewMcqItem mcqItem = new NewMcqItem();
			mcqItem.RequireAtLeastOne = true;

			Assert.AreEqual(mcqXmlWithRequiredAttributeSetToTrue, mcqItem.ToXml("Q1"));
		}

		[Test]
		public void McqWithRequiredAttributeSetToTrueRegeneratesCorrectlyFromXml()
		{
			IMcqItem mcqItem = new NewMcqItem(new XmlElement(mcqXmlWithRequiredAttributeSetToTrue));

			Assert.AreEqual(true, mcqItem.RequireAtLeastOne);
		}

		private readonly string mcqXmlWithSelectOnylOneAttributeSetToFalse =
			@"<mc label=""Q1"" onlyone=""false"" required=""false"" style=""vertical"">" +
			@"<question>" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"<font face=""Arial"" size=""200"" color=""000000"">" +
			@"[Replace this with your question. Use Enter key to add choices below.]" +
			@"</font>" +
			@"</paragraph>" +
			@"</question>" +
			@"<choice label=""a"">" +
			@"</choice>" +
			@"</mc>" + Environment.NewLine;

		[Test]
		public void McqWithSelectOnylOneAttributeSetToFalseGeneratesExpectedMcqXml()
		{
			NewMcqItem mcqItem = new NewMcqItem();
			mcqItem.SelectOnlyOne = false;

			Assert.AreEqual(mcqXmlWithSelectOnylOneAttributeSetToFalse, mcqItem.ToXml("Q1"));
		}

		[Test]
		public void McqWithSelectOnylOneAttributeSetToFalseRegeneratesCorrectlyFromXml()
		{
			IMcqItem mcqItem = new NewMcqItem(new XmlElement(mcqXmlWithSelectOnylOneAttributeSetToFalse));

			Assert.AreEqual(false, mcqItem.SelectOnlyOne);
		}

		[Test]
		public void ToStringReturnsQualifiedFieldName()
		{
			NewMcqItem mcqItem = new NewMcqItem();
			IForm form = Project.Current.AddForm();
			form.ItemList.Add(mcqItem);

			Assert.AreEqual(mcqItem.QualifiedFieldName, mcqItem.ToString());
		}

		private static string projectWithDynamicMcqXml =
			@"<project name=""Test"" themePath=""default"" format=""" + Project.XmlFormatVersion + @""" designerBuild=""0"">" + Environment.NewLine +
			@"<pageHeader></pageHeader>" +
			@"<forms>" + Environment.NewLine +
			@"<form name=""Form 1"" startPoint=""true""><items>" + Environment.NewLine +
			@"<fib label=""Q1"" style=""topLabels""><paragraph indent=""0"" align=""left"">" +
			@"[Replace this with your question. Underscores create blanks.] " +
			@"<blank label=""a"" length=""20"" height=""1"" required=""false""/></paragraph></fib>" + Environment.NewLine +
			@"<mc label=""Q2"" onlyone=""true"" required=""false"" style=""vertical"">" +
			@"<question><paragraph " +
			@"indent=""0"" align=""left""><font face=""Arial"" size=""200"" color=""000000"">[Replace this with " +
			@"your question. Use Enter key to add choices below.]</font></paragraph></question>" +
			@"<data-provider>" +
			@"<dynamic-mcq version=""1"">" +
			@"<display-expression>" +
			@"<field name=""Record:Form 1:Q1:a""/>" + Environment.NewLine +
			@"</display-expression>" +
			@"<value-expression>" +
			@"<field name=""Record:Form 1:Q1:a""/>" + Environment.NewLine +
			@"</value-expression>" +
			@"<sort-expression>" +
			@"<field name=""Record:Form 1:Q1:a""/>" + Environment.NewLine +
			@"</sort-expression>" +
			@"<record-selector>" +
			@"<form name=""Form 1"" />" +
			@"</record-selector>" +
			@"</dynamic-mcq>" +
			@"</data-provider>" +
			@"</mc>" + Environment.NewLine +
			@"</items>" + Environment.NewLine +
			@"</form></forms>" + Environment.NewLine +
			@"</project>" + Environment.NewLine;

		[Test]
		public void DynamicMcqRegeneratesCorrectlyFromXml()
		{
			Project.Create(new XmlElement(projectWithDynamicMcqXml));

			Assert.AreEqual(projectWithDynamicMcqXml, Project.Current.ToXml());
		}

		[Test]
		public void DynamicMcqDataSourceFunctionIsNotNullWhenProjectCreatedFromXml()
		{
			Project.Create(new XmlElement(projectWithDynamicMcqXml));

			IForm form = Project.Current.FormList[0];
			IMcqItem mcqItem = form.ItemList[1] as IMcqItem;

			Assert.IsNotNull(mcqItem.DataSourceFunction, "DataSourceFunction property is null");
		}

		[Test]
		public void CanSetDataSourceFunction()
		{
			IMcqItem mcqItem = new NewMcqItem();
			mcqItem.ChoiceContents = new DataProvider(FunctionLoader.Current.Functions["dynamic-mcq"].CreateInstance());

			Assert.IsNotNull(mcqItem.DataSourceFunction, "DataSourceFunction property is null");
			Assert.IsTrue(Project.FunctionMapById.ContainsKey(mcqItem.DataSourceFunction.InstanceId));
		}

		[Test]
		public void CanClearDataSourceFunction()
		{
			IMcqItem mcqItem = new NewMcqItem();

			mcqItem.ChoiceContents = new DataProvider(FunctionLoader.Current.Functions["dynamic-mcq"].CreateInstance());
			Assert.IsNotNull(mcqItem.DataSourceFunction, "DataSourceFunction property is null");

			mcqItem.ChoiceContents = new DataProvider();
			Assert.IsNull(mcqItem.DataSourceFunction);
		}
	}
}
