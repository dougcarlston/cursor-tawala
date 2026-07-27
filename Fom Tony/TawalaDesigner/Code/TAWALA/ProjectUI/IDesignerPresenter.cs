// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.

using System;
using System.Windows.Forms;

namespace Tawala.ProjectUI
{
	public interface IDesignerPresenter
	{
		void Initialize();

		void UpdateCaption();

		void StartInitializationTasks(Action<bool> finished, MethodInvoker progress);
		bool InitializationTasksCompleted();

		void LaunchProjectManager();

		string ProjectFullPath { get; }
		string ProjectDefaultDirectory { get; }

		void NewProject();
		bool SaveProject(bool saveAs);
		DialogResult SaveProjectIfModified();
		void OpenProject();

		void UpdateFieldProviderInfo();

		void DeployAsync();
		void UpdateDeploymentInfoAsync();

		bool IsBackgroundTaskQueueBusy { get; }

		bool ValidateProject(string path);
	}
}
