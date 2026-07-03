// $Workfile: MDIComponentView.cs $
// $Revision: 19 $	$Date: 9/04/07 10:01a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using Tawala.Projects;
using System.Diagnostics;
using Tawala.Projects.Components;

namespace Tawala.ProjectUI
{
	/// <summary>
	/// Base class for a MDI child that displays a Project Component
	/// </summary>
	/// <remarks>A derived classes should set the Tag property to their Component</remarks>
	public class MDIComponentView : System.Windows.Forms.Form
	{
		private string originalCaption = string.Empty;
		private Component projComponent;

		public virtual ComponentPalette Palette
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// When a derived class is being designed
		/// </summary>
		protected MDIComponentView()
		{
		}

		/// <summary>
		/// Constructor that is the normal pathway for derived classes
		/// </summary>
		protected MDIComponentView(Component comp)
		{
			Tag = projComponent = comp;
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();

			// Make sure no one else messed with it during initialization
			Tag = projComponent;

			DoubleBuffered = true;
			ResizeRedraw = true;

			originalCaption = Text;

			if (projComponent != null) // null if derived class being designed
			{
				Text = originalCaption + projComponent.Name;
				Project.Events.ComponentRenamed += project_ComponentRenamed;
			}
		}

		public virtual bool CanPrint
		{
			get { return false; }
		}

		public virtual void PrintPreview()
		{
		}

		public virtual void Print()
		{
		}

		public virtual PrintDocument PrintDocument
		{
			get
			{
				return null;
			}
		}

		private void project_ComponentRenamed(object sender, ComponentRenamedEventArgs e)
		{
			if (e.Component == projComponent)
			{
				Text = originalCaption + e.Component.Name;
			}
		}
		
		protected override void OnHandleDestroyed(EventArgs e)
		{
			projComponent = null;
			Tag = null;
			base.OnHandleDestroyed(e);
		}
	}
}
