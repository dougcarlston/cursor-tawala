// $Workfile: FormNameSource.cs $
// $Revision: 6 $	$Date: 2/25/08 4:49p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Tawala.Common;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace Tawala.Dialogs
{
	public class FormNameSource : Collection<IForm>, INotifyPropertyChanged
	{
		private string projectName = "";

		public string ProjectName
		{
			get
			{
				return projectName;
			}
			set
			{
				projectName = value;
				notify("ProjectName");
			}
		}

		public FormNameSource()
		{
			Project.Events.FormChanged += Events_FormChanged;
			Project.Events.ComponentAdded += Events_ComponentAdded;
			Project.Events.ComponentRemoved += Events_ComponentRemoved;
			Project.Events.ComponentRenamed += Events_ComponentRenamed;
		}

		void Events_ComponentAdded(object sender, ComponentEventArgs e)
		{
			if (e.Component is Form)
			{
				notify("ComponentAdded");
			}
		}

		void Events_ComponentRemoved(object sender, ComponentEventArgs e)
		{
			if (e.Component is Form)
			{
				notify("ComponentRemoved");
			}
		}

		void Events_ComponentRenamed(object sender, ComponentRenamedEventArgs e)
		{
			if (e.Component is Form)
			{
				notify("ComponentRenamed");
			}
		}

		void Events_FormChanged(object sender, ComponentEventArgs e)
		{
			notify("FormChanged");
		}

		private bool projectNameReferencesCurrentProject()
		{
			return (projectName == Project.Current.Name || projectName == "(Current Project)");
		}

		public void Update()
		{
			if (projectName != "")
			{
				Clear();

				if (projectNameReferencesCurrentProject())
				{
					foreach (Form form in Project.Current.FormList)
					{
						Add(form);
					}
				}
				else
				{
					Deployments deployments = DeploymentInfo.Projects;

					if (deployments.ContainsKey(projectName))
					{
						StartingPoints startPoints = DeploymentInfo.Projects[projectName];

						foreach (string formName in startPoints.Keys)
						{
							Add(new FormInfo(formName));
						}
					}
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
