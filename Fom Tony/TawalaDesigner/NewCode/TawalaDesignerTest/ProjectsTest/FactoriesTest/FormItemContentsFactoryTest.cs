using System;
using NUnit.Framework;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Factories;
using TawalaTest.TestingSupport;
using Tawala.XmlSupport;

namespace TawalaTest.ProjectTest.FactoriesTest
{
	[TestFixture]
	public class FormItemContentsFactoryTest
	{
		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		[Test]
		public void CanMakeDataProviderFromXml()
		{
			string dataProviderXml =
				@"<data-provider>" + Environment.NewLine +
				@"<dynamic-mcq version=""1"">" + Environment.NewLine +
				@"<display-expression>" + Environment.NewLine +
				@"<field name=""Record:Form 1:Q1:a""/>" + Environment.NewLine +
				@"</display-expression>" + Environment.NewLine +
				@"<value-expression>" + Environment.NewLine +
				@"<field name=""Record:Form 1:Q1:a""/>" + Environment.NewLine +
				@"</value-expression>" + Environment.NewLine +
				@"<sort-expression>" + Environment.NewLine +
				@"<field name=""Record:Form 1:Q1:a""/>" + Environment.NewLine +
				@"</sort-expression>" + Environment.NewLine +
				@"<record-selector>" + Environment.NewLine +
				@"<form name=""Form 1""/>" + Environment.NewLine +
				@"</record-selector>" + Environment.NewLine +
				@"</dynamic-mcq>" + Environment.NewLine +
				@"</data-provider>" + Environment.NewLine;

			DataProvider dataProvider = FormItemContentsFactory.MakeObject(new XmlElement(dataProviderXml)) as DataProvider;

			Assert.IsNotNull(dataProvider);
		}

		[Test]
		public void CanMakeDynamicMcqFunctionReferenceFromXml()
		{
			string functionReferenceXml =
				@"<dynamic-mcq version=""1"">" + Environment.NewLine +
				@"<display-expression>" + Environment.NewLine +
				@"<field name=""Record:Form 1:Q1:a""/>" + Environment.NewLine +
				@"</display-expression>" + Environment.NewLine +
				@"<value-expression>" + Environment.NewLine +
				@"<field name=""Record:Form 1:Q1:a""/>" + Environment.NewLine +
				@"</value-expression>" + Environment.NewLine +
				@"<sort-expression>" + Environment.NewLine +
				@"<field name=""Record:Form 1:Q1:a""/>" + Environment.NewLine +
				@"</sort-expression>" + Environment.NewLine +
				@"<record-selector>" + Environment.NewLine +
				@"<form name=""Form 1""/>" + Environment.NewLine +
				@"</record-selector>" + Environment.NewLine +
				@"</dynamic-mcq>";

			FunctionReference functionReference = FormItemContentsFactory.MakeObject(new XmlElement(functionReferenceXml)) as FunctionReference;

			Assert.IsNotNull(functionReference);
			Assert.IsNotNull(functionReference.Function);
		}
	}
}
