// $Workfile: InvitatonBindingSources.cs $
// $Revision: 3 $	$Date: 2/28/08 3:45p $
// Copyright © 2005 - 2006 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Tawala.Projects;

namespace Tawala.Dialogs
{
	internal class NameBindingSource<T> : BindingSource
			where T : System.Collections.IList, INotifyPropertyChanged, new()
	{
		protected T names = new T();

		public NameBindingSource()
		{
			DataSource = names;
			names.PropertyChanged += delegate { ResetBindings(true); };
		}

		protected void Init(ComboBox cb, string find)
		{
			int pos = -1;

			for (int i = 0; i < Count; ++i)
			{
				string name = this[i].ToString();
				if (name.CompareTo(find) == 0)
				{
					pos = i;
					break;
				}
			}

			if (pos >= 0)
				Position = pos;
			cb.DataSource = this;
		}
	}

	internal class ProjectNameBindingSource : NameBindingSource<ProjectNameSource>
	{
		public ProjectNameBindingSource(ComboBox cb, string findProj)
		{
			Init(cb, findProj);
		}
	}

	internal class FormNameBindingSource : NameBindingSource<FormNameSource>
	{
		public FormNameBindingSource(ComboBox cb, ProjectNameBindingSource projectSource, string findForm)
		{
			projectSource.CurrentItemChanged += delegate(object sender, EventArgs e) { setCurrentProject(sender); };
			setCurrentProject(projectSource);
			Init(cb, findForm);
		}

		private void setCurrentProject(object projectSource)
		{
			string name = ((ProjectNameBindingSource)projectSource).Current as string;
			if (!string.IsNullOrEmpty(name))
			{
				names.ProjectName = name;
			}
		}
	}
}
