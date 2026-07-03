// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Interfaces;

namespace Tawala.DocumentDesigner
{
	[System.Runtime.InteropServices.ComVisible(true)]
	public class DocumentScriptingObject : Browser.BrowserScriptingObject
	{
		public DocumentScriptingObject(IDocumentView view)
		{
			View = view;
		}

		public IDocumentView View
		{
			get;
			private set;
		}
	}

}
