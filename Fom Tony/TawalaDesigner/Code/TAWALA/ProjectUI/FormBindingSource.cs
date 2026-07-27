using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Tawala.Projects;

namespace Tawala.ProjectUI
{
	// Provides a BindingSource for Forms that automatically converts Project Form events to BindingSource changes.
	//
	// Here, at the lowest level UI Project as it requires System.Windows.Form.
	// It would've really been nice if BindingSource originated from System.ComponentModel

	public class FormBindingSource : BindingSource
	{
		public FormBindingSource()
		{
			DataSource = Project.Current.FormList;

			Project.Events.ComponentAdded += formListChanged;
			Project.Events.ComponentRemoved += formListChanged;
			Project.Events.ComponentRenamed += formListChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Project.Events.ComponentAdded -= formListChanged;
				Project.Events.ComponentRemoved -= formListChanged;
				Project.Events.ComponentRenamed -= formListChanged;
			}
			base.Dispose(disposing);
		}

		private void formListChanged(object sender, ComponentEventArgs e)
		{
			if (e.Component is Projects.Form)
			{
				ResetBindings(false);
			}
		}
	}
}
