using System;
using System.Collections.ObjectModel;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Functions.Runtime;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Function;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
    [TestFixture]
	public class ForwardReferenceInFunctionConditionsCausesBadSave689
    {
		private string projectXmlStart =
			"<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine +
			"<project name=\"ForwardReference01\" themePath=\"default\" format=\"1.9\" designerBuild=\"186\">" + Environment.NewLine +
			"<forms>" + Environment.NewLine +
			"<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\">" + Environment.NewLine +
			"<items>" + Environment.NewLine +
			"<fib label=\"Q1\" style=\"topLabels\">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			"<tabPositions><tabStop position=\"2880\"/></tabPositions>" +
			"<font face=\"Arial\" size=\"200\" color=\"000000\"> FIB </font>" +
            "<blank label=\"a\" length=\"20\" required=\"false\"></blank>" +
			"</paragraph>" +
			"</fib>" + Environment.NewLine +
			"<text label=\"T1\"" + XmlConstants.DefaultTextItemStyleAttribute + ">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			"<tabPositions><tabStop position=\"2880\"/></tabPositions>" +
			"<font>";

		private string functionXmlStart =
			"<record-count version=\"3\">" +
			"<form-name>Form 1</form-name>" +
			"<conditions><form name=\"Form 1\" />";

		private string functionXmlEnd =
			"</conditions>" +
			"</record-count>";

		private string projectXmlEnd =
			"</font></paragraph></text>" + Environment.NewLine +
			"<field name=\"Field1\"/>" + Environment.NewLine +
			"</items>" + Environment.NewLine +
			"</form>" + Environment.NewLine +
			"</forms>" + Environment.NewLine +
			"</project>";

		// forward references to Hidden Field in the Field (left-hand) component of the condition:
		private string forwardReferenceToFieldInConditionField =
			"<conditions>" + Environment.NewLine +
			"<equals field=\"Form 1:Field1\">" + Environment.NewLine +
			"<string field=\"Form 1:Q1:a\"/>" + Environment.NewLine +
			"</equals>" + Environment.NewLine +
			"</conditions>";

		private string forwardReferenceToRecordFieldInConditionField =
			"<conditions>" + Environment.NewLine +
			"<equals field=\"Record:Form 1:Field1\">" + Environment.NewLine +
			"<string field=\"Form 1:Q1:a\"/>" + Environment.NewLine +
			"</equals>" + Environment.NewLine +
			"</conditions>";

		[Test]
		public void ForwardReferenceToFieldInConditionFieldIsResolved()
		{
			string projectXml =
				projectXmlStart + functionXmlStart +
				forwardReferenceToFieldInConditionField +
				functionXmlEnd + projectXmlEnd;

			Condition condition = openProjectAndGetFunctionCondition(projectXml);

			Assert.AreEqual("Form 1:Field1 equals Form 1:Q1:a", condition.ToString());

			HiddenField forwardReferencedField = Project.Current.FormList[0].ItemList[2] as HiddenField;
			Assert.AreSame(forwardReferencedField, condition.Field);
		}

		[Test]
		public void ForwardReferenceToRecordFieldInConditionFieldIsResolved()
		{
			string projectXml =
				projectXmlStart + functionXmlStart +
				forwardReferenceToRecordFieldInConditionField +
				functionXmlEnd + projectXmlEnd;

			Condition condition = openProjectAndGetFunctionCondition(projectXml);
			
			Assert.AreEqual("Record:Form 1:Field1 equals Form 1:Q1:a", condition.ToString());
		}

		// forward references to Hidden Field in the Expression (right-hand) component of the condition:
		private string forwardReferenceToFieldInConditionExpression =
			"<conditions>" + Environment.NewLine +
			"<equals field=\"Form 1:Q1:a\">" + Environment.NewLine +
			"<string field=\"Form 1:Field1\"/>" + Environment.NewLine +
			"</equals>" + Environment.NewLine +
			"</conditions>";

		private string forwardReferenceToRecordFieldInConditionExpression =
			"<conditions>" + Environment.NewLine +
			"<equals field=\"Form 1:Q1:a\">" + Environment.NewLine +
			"<string field=\"Record:Form 1:Field1\"/>" + Environment.NewLine +
			"</equals>" + Environment.NewLine +
			"</conditions>";

		[Test]
		public void ForwardReferenceToFieldInConditionExpressionIsResolved()
		{
			string projectXml =
				projectXmlStart + functionXmlStart +
				forwardReferenceToFieldInConditionExpression +
				functionXmlEnd + projectXmlEnd;

			Condition condition = openProjectAndGetFunctionCondition(projectXml);

			Assert.AreEqual("Form 1:Q1:a equals Form 1:Field1", condition.ToString());

			HiddenField forwardReferencedField = Project.Current.FormList[0].ItemList[2] as HiddenField;

			FieldElement fieldElement = condition.Expression.Elements[0] as FieldElement;
			Assert.AreSame(forwardReferencedField, fieldElement.Field);
		}

		[Test]
		public void ForwardReferenceToRecordFieldInConditionExpressionIsResolved()
		{
			string projectXml =
				projectXmlStart + functionXmlStart +
				forwardReferenceToRecordFieldInConditionExpression +
				functionXmlEnd + projectXmlEnd;

			Condition condition = openProjectAndGetFunctionCondition(projectXml);

			Assert.AreEqual("Form 1:Q1:a equals Record:Form 1:Field1", condition.ToString());
		}

		private Condition openProjectAndGetFunctionCondition(string projectXml)
		{
			try
			{
				Util.OpenProjectXml(projectXml);
			}
			catch (Exception e)
			{
				Assert.Fail(e.Message);
			}

			IForm form = Project.Current.FormList[0];
			TextItem textItem = form.ItemList[1] as TextItem;

			DocumentFunctionField functionField = getFunctionFromTextItem(textItem);

			return getSingleConditionFromFunction(functionField);
		}

		private DocumentFunctionField getFunctionFromTextItem(TextItem textItem)
		{
			FormItemParagraph paragraph = textItem.Contents[0] as FormItemParagraph;

			ParagraphInlineComponent paragraphInlineComponent = paragraph.Contents[0] as ParagraphInlineComponent;
	
			return paragraphInlineComponent.Contents as DocumentFunctionField;
		}

		private static Condition getSingleConditionFromFunction(DocumentFunctionField functionField)
		{
			IFunction function = Project.FunctionMapById[functionField.FunctionInstanceId];

            FunctionFilterConditions functionConditions = function.Info.Parameters[1].GetValue(function) as FunctionFilterConditions;

			return functionConditions.Conditions[0] as Condition;
		}
	}
}