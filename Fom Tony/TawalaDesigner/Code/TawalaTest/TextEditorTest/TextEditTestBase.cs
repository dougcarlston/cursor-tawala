// $Workfile: TextEditTestBase.cs $
// $Revision: 9 $	$Date: 3/16/07 2:17p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.TextEditor;

namespace TawalaTest.TextEditorTest
{
	public class TextEditTestBase
	{
		protected TestForm form = null;
		protected TextEdit editor = null;
		protected TXTextControl.TextControl txTextControl = null;

		[SetUp]
		public void MethodSetup()
		{
			Clipboard.Clear();

			form = new TestForm();

			Assert.AreEqual(1, form.Controls.Count);
			Assert.IsTrue(form.Controls[0] is TextEdit);

			editor = form.Controls[0] as TextEdit;

			form.Show();

			FieldInfo f = editor.GetType().GetField("txTextControl", BindingFlags.NonPublic | BindingFlags.Instance);
			txTextControl = (TXTextControl.TextControl)f.GetValue(editor);
		}

		[TearDown]
		public void MethodTearDown()
		{
			editor = null;
			form.Close();
			form = null;
			txTextControl = null;
		}
	}
}
