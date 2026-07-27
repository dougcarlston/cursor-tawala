// $Workfile: FieldPaletteEventRouter.cs $
// $Revision: 3 $	$Date: 6/04/07 2:04p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Tawala.Common;

namespace Tawala.ProjectUI
{
	public class FieldPaletteEventRouter
	{
		private TreeNodeMouseClickEventHandler handler;

		internal void SetDoubleClick(TreeNodeMouseClickEventHandler newHandler)
		{
			unhookHandlerTarget();
			hookHandlerTarget(newHandler);
		}

		internal void RemoveDoubleClick(TreeNodeMouseClickEventHandler removeHandler)
		{
			if (handler != null && handler.Equals(removeHandler))
			{
				unhookHandlerTarget();
			}
		}

		internal void InvokeDoubleClick(object sender, TreeNodeMouseClickEventArgs args)
		{
			TreeNodeMouseClickEventHandler validatedHandler = validateHandler();
			if (validatedHandler != null)
			{
				validatedHandler(sender, args);
			}
		}

		internal FieldPaletteEventRouter() { }

		private void hookHandlerTarget(TreeNodeMouseClickEventHandler newHandler)
		{
			var targetControl = newHandler.Target as Control;
			if (targetControl != null)
			{
				targetControl.ParentChanged += new EventHandler(targetControl_ParentChanged);
				targetControl.HandleDestroyed += targetControl_HandleDestroyed;
			}

			handler = newHandler;
		}

		private void unhookHandlerTarget()
		{
			if (handler == null)
				return;

			Control targetControl = handler.Target as Control;
			if (targetControl != null)
			{
				targetControl.HandleDestroyed -= targetControl_HandleDestroyed;
				targetControl.ParentChanged -= targetControl_ParentChanged;
			}

			handler = null;
		}

		private TreeNodeMouseClickEventHandler validateHandler()
		{
			if (handler == null)
				return null;

			return handler;
		}

		void targetControl_HandleDestroyed(object sender, EventArgs e)
		{
			Control c = getControl(sender);

			if (c != null)
				unhookHandlerTarget();
		}

		void targetControl_ParentChanged(object sender, EventArgs e)
		{
			Control c = getControl(sender);

			if (c != null && c.Parent == null)
				unhookHandlerTarget();
		}

		private Control getControl(object sender)
		{
			Control control = sender as Control;

			Debug.Assert(control != null);
			Debug.Assert(handler != null);
			Debug.Assert(handler.Target == control);

			if (handler == null || handler.Target != control)
				return null;

			return control;
		}
	}
}