// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using Tawala.Common;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace Tawala.Dialogs
{
    internal class NameBindingSource<T> : BindingSource
        where T : IList, INotifyPropertyChanged, new()
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
            {
                Position = pos;
            }
            cb.DataSource = this;
        }
    }

    internal class ProjectNameSource : Collection<string>, INotifyPropertyChanged
    {
        public ProjectNameSource()
        {
            Project.Events.DeploymentInfoChanged += Events_DeploymentInfoChanged;
            Project.Events.ProjectOpened += Events_ProjectOpened;
            Project.Events.SaveProject += Events_SaveProject;
            Update();
        }

        private void Events_DeploymentInfoChanged(object sender, EventArgs e)
        {
            notify("DeploymentInfoChanged");
        }

        private void Events_ProjectOpened(object sender, ProjectEventArgs e)
        {
            notify("OpenProject");
        }

        private void Events_SaveProject(object sender, SaveProjectEventArgs e)
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

    internal class ProjectNameBindingSource : NameBindingSource<ProjectNameSource>
    {
        public ProjectNameBindingSource(ComboBox cb, string findProj)
        {
            Init(cb, findProj);
        }
    }

    internal class FormNameSource : Collection<IForm>, INotifyPropertyChanged
    {
        private string projectName = "";

        public FormNameSource()
        {
            Project.Events.FormChanged += project_FormChanged;
            Project.Events.ComponentAdded += project_ComponentAdded;
            Project.Events.ComponentRemoved += project_ComponentRemoved;
            Project.Events.ComponentRenamed += project_ComponentRenamed;
        }

        public string ProjectName
        {
            get { return projectName; }
            set
            {
                projectName = value;
                notify("ProjectName");
            }
        }

        private void project_ComponentAdded(object sender, ComponentEventArgs e)
        {
            if (e.Component is IForm)
            {
                notify("ComponentAdded");
            }
        }

        private void project_ComponentRemoved(object sender, ComponentEventArgs e)
        {
            if (e.Component is IForm)
            {
                notify("ComponentRemoved");
            }
        }

        private void project_ComponentRenamed(object sender, ComponentRenamedEventArgs e)
        {
            if (e.Component is IForm)
            {
                notify("ComponentRenamed");
            }
        }

        private void project_FormChanged(object sender, ComponentEventArgs e)
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
                    foreach (IForm form in Project.Current.FormList)
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
            var name = ((ProjectNameBindingSource)projectSource).Current as string;
            if (!string.IsNullOrEmpty(name))
            {
                names.ProjectName = name;
            }
        }
    }
}