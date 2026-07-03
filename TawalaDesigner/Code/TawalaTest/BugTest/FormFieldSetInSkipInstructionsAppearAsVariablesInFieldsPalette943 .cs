using System;
using System.Windows.Forms;
using Tawala.ProjectUI;
using NUnit.Framework;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
	/// <summary>
	/// Test for Mantis issue 943 (Assigning Form Field in Skip Instructions adds bogus Variables upon reopening Project).
	/// </summary>
	[TestFixture]
	public class FormFieldSetInSkipInstructionsAppearAsVariablesInFieldsPalette943
	{
		private string projectXml =
			@"<?xml version=""1.0"" encoding=""utf-8"" ?>" + Environment.NewLine +
			@"<project name=""yyy"" themePath=""default"" format=""1.13"" designerBuild=""230"">" + Environment.NewLine +
			@"<pageHeader></pageHeader><forms>" + Environment.NewLine +
			@"<form name=""Form 1"" startPoint=""true"" themePath=""default"">" + Environment.NewLine +
			@"<items>" + Environment.NewLine +
			@"<fib label=""Q1"" style=""topLabels"">" +
			@"<paragraph indent=""0"" align=""left"">" +
			@"<tabPositions><tabStop position=""2880""/></tabPositions>" +
			@"<font face=""Arial"" size=""200"" color=""000000"">Name: </font>" +
            @"<blank label=""a"" length=""20"" required=""false""></blank>" +
			@"</paragraph>" +
			@"</fib>" + Environment.NewLine +
			@"<skipInstructions>" + Environment.NewLine +
			@"<set field=""Form 1:Q1:a"" arithmeticAsText=""false"">" + Environment.NewLine +
			@"<string value=""blah""/>" + Environment.NewLine +
			@"</set>" + Environment.NewLine +
			@"</skipInstructions>" + Environment.NewLine +
			@"</items>" + Environment.NewLine +
			@"</form>" + Environment.NewLine +
			@"</forms>" + Environment.NewLine +
			@"</project>";

		[Test]
		public void FieldSetInSkipInstructionsDoesNotAddVariableToFieldsPalette()
		{
			Util.OpenProjectXml(projectXml);

			var windowsForm = new System.Windows.Forms.Form();

			var fieldsPalette = new FieldsPalette();

			windowsForm.Controls.Add(fieldsPalette);
			windowsForm.Show();

			fieldsPalette.RefreshFieldList();

			Assert.AreEqual(2, fieldsPalette.FieldsTreeView.Nodes.Count);
			TreeNode variablesNode = fieldsPalette.FieldsTreeView.Nodes[1];

			Assert.AreEqual(1, variablesNode.Nodes.Count);
			Assert.AreEqual("_InviteeID", variablesNode.Nodes[0].Text);
		}
	}
}
