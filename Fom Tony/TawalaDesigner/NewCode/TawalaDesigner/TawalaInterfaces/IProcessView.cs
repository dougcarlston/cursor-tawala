// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Windows.Forms;
using Tawala.Processes;
using Tawala.Projects.Processes;

namespace Tawala.Interfaces
{
	public interface IProcessView
	{
		IProcessPresenter Presenter { get; set; }
		IApplicationView ParentView { get; }
		Form MdiParent { get; set; }
		void Show();
		void Activate();
		void Close();
		event EventHandler Activated;

		ProcessEditor ProcessEditor { get; }
		IProcess Process { get; }

		void SetProcessName(string processName);
	}
}
