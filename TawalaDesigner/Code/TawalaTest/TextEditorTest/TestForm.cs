// $Workfile: TestForm.cs $
// $Revision: 5 $	$Date: 3/16/07 2:17p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Tawala.TextEditor;

namespace TawalaTest.TextEditorTest
{
	public class TestForm : Form
	{
		private TextEdit editor = null;

		internal TestForm()
		{
			ClientSize = new Size(400, 400);

			editor = new TextEdit();
			editor.Name = "TextEdit";
			editor.Dock = DockStyle.Fill;

			Controls.Add(editor);

		}
	}
}
