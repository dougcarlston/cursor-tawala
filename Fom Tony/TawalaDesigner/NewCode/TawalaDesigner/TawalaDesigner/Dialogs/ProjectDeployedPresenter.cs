// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Tawala.Common;

namespace TawalaDesigner.Dialogs
{
	public class ProjectDeployedPresenter : IProjectDeployedPresenter
	{
		private IProjectDeployedView view;

		public ProjectDeployedPresenter(IProjectDeployedView view)
		{
			this.view = view;
		}

		#region IProjectDeployedPresenter Members

		public void CloseRequested()
		{
			view.Hide();
		}

		public void FormViewRequested(string urlString)
		{
			Process process = new Process();
			process.StartInfo.FileName = RegistryHelper.GetDefaultBrowser();
			process.StartInfo.Arguments = urlString;
			process.Start();
		}

		#endregion
	}
}
