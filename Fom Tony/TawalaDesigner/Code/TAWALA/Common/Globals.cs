// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace Tawala.Common
{
	public static class Globals
	{
		public static void CloseAllOpenDialogs()
		{
			var openDialogs = new Collection<Form>();

			foreach (Form form in Application.OpenForms)
			{
				if (form.Name == "ConfigureFunctionDialog")
				{
					openDialogs.Add(form);
				}
			}

			foreach (var dialog in openDialogs)
			{
				dialog.Close();
			}
		}
	}
}
