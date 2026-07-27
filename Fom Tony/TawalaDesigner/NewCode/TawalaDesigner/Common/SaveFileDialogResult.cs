// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System.Windows.Forms;

namespace Tawala.Common
{
	public class SaveFileDialogResult
	{
		public SaveFileDialogResult(DialogResult dialogResult, string fileName)
		{
			this.DialogResult = dialogResult;
			this.FileName = fileName;
		}

		public DialogResult DialogResult { get;  private set; }
		public string FileName { get; private set; }
	}
}
