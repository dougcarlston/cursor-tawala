using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;

using Tawala.Controls;
using Tawala.Projects;
using Tawala.ProjectUI;
using TawalaTest.TestSupport;

using Tawala.Documents;
using Tawala.XmlSupport;
using Tawala.RtfSupport;
using Tawala.Processes;


namespace TawalaTest.BugTest
{
	[TestFixture]
	public class InsertFunctionMustReplaceSelection575
	{
		private System.Windows.Forms.Form form = null;
		private DocumentEditor editor = null;

		[SetUp]
		public void SetUp()
		{
			TestSupport.Util.NewTestProject();

			form = new System.Windows.Forms.Form();
			editor = new DocumentEditor();
			form.Controls.Add(editor);

			form.Show();
			editor.SetRTF(RtfConstants.DocumentPrologue + " This is the first paragraph.\\par }");
		}

		[Test]
		public void InsertFunctionReplacesSelection()
		{
			editor.Select(12, 5);

			Assert.AreEqual(editor.Selection.Start, 12);
			Assert.AreEqual(editor.Selection.Length, 5);
			Assert.AreEqual(editor.Selection.Text, "first");

			editor.InsertFunctionField(123456, "<<FUNCTION>>");

			Assert.AreEqual(editor.Selection.Start, 24);
			Assert.AreEqual(editor.Selection.Length, 0);

			editor.SelectAll();

			Assert.AreEqual(editor.Selection.Text, "This is the <<FUNCTION>> paragraph.");
		}
	}
}
