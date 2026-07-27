// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System.Windows.Forms;
using Tawala.Interfaces;
using Tawala.Dialogs;
using Tawala.Projects;

namespace Tawala.MainApplication
{
	public static class ApplicationPresenter
	{
		static IApplicationView applicationView;

		public delegate void DropDownOpenedEventHandler(object sender, DropDownOpenedEventArgs e);
		public static event DropDownOpenedEventHandler EditDropDownOpened;

		public static IApplicationView View
		{
			get { return applicationView; }
			set { applicationView = value; }
		}

		public static System.Windows.Forms.Form MainApplicationForm
		{
			get { return applicationView as System.Windows.Forms.Form; }
		}

		public static Control DesignerPalette
		{
			get { return applicationView != null ? applicationView.ComponentPalette : null; }
		}

		public static void RaiseEditDropDownOpenedEvent(object sender, DropDownOpenedEventArgs e)
		{
			if (EditDropDownOpened != null)
			{
				EditDropDownOpened(sender, e);
			}
		}

		public static bool ProjectIsDeployable()
		{
			return Project.Current.FormList.Count > 0;
		}

		public static void SetProjectNameInTitleBar(string projectName)
		{
			if (applicationView != null)
			{
				applicationView.SetProjectNameInTitleBar(projectName);
			}
		}
	}
}
