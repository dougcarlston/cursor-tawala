// $Workfile: ProjectNameSource.cs $
// $Revision: 7 $	$Date: 2/16/08 7:22p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Tawala.Common;
using Tawala.Projects;

namespace Tawala.Dialogs
{
	public class ProjectNameSource : Collection<string>, INotifyPropertyChanged
	{
		public ProjectNameSource()
		{
			Project.Events.DeploymentInfoChanged += Events_DeploymentInfoChanged;
			Project.Events.ProjectOpened += Events_ProjectOpened;
			Project.Events.SaveProject += Events_SaveProject;
			Update();
		}

		void Events_DeploymentInfoChanged(object sender, EventArgs e)
		{
			notify("DeploymentInfoChanged");
		}

		void Events_ProjectOpened(object sender, ProjectEventArgs e)
		{
			notify("OpenProject");
		}

		void Events_SaveProject(object sender, SaveProjectEventArgs e)
		{
			notify("SaveProject");
		}

		public void Update()
		{
			Clear();

			Add("(Current Project)");

			Deployments deployments = DeploymentInfo.Projects;

			foreach (string projName in deployments.Keys)
			{
				if (projName != Project.Current.Name)
				{
					Add(projName);
				}
			}
		}

		//private void disableListChangedEvents()
		//{
		//    this.RaiseListChangedEvents = false;
		//}

		//private void enableListChangedEvents()
		//{
		//    this.RaiseListChangedEvents = true;
		//    this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
		//}

		private void notify(string info)
		{
			Update();

			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		#region INotifyPropertyChanged Interface

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
