// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Text;
using System.Xml;

using Tawala.Common;
using Tawala.Interfaces;
using Tawala.Projects;

namespace TawalaDesigner.Dialogs
{
    public class StartupTasksPresenter : IStartupTasksPresenter
    {
        internal StartupTasksPresenter(IStartupTasksView view)
        {
            View = view;

            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

            taskQueue.Enqueue(new LoadFunctionsTask());
            taskQueue.Enqueue(new GetThemesTask());
            taskQueue.Enqueue(new GetSharedDataSourcesTask());
        }

        #region IStartupTasksPresenter Members

        public void DoAsynchronousTasks()
        {
            started = DateTime.Now;

            processQueue();
        }

        #endregion

        internal IStartupTasksView View
        {
            get;
            set;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = e.Argument;
            ((IBackgroundTask)e.Argument).PerformTaskOnBackgroundThread();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ((IBackgroundTask)e.Result).TaskCompleted();

            processQueue();
        }

        private void processQueue()
        {
            if (taskQueue.Count > 0)
            {
                IBackgroundTask task = taskQueue.Dequeue();
                View.SetTaskDescription(task.Description);
                backgroundWorker.RunWorkerAsync(task);
            }
            else
            {
                allWorkCompleted();
            }
        }

        private void allWorkCompleted()
        {
            backgroundWorker.Dispose();
            backgroundWorker = null;

            Debug.WriteLine(string.Format("BackgroundWorker took {0}ms to complete", (DateTime.Now - started).TotalMilliseconds));

            View.TasksCompleted();
        }

        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private Queue<IBackgroundTask> taskQueue = new Queue<IBackgroundTask>();
        private DateTime started;

        private class LoadFunctionsTask : IBackgroundTask
        {
            #region IBackgroundTask Members

            public string Description
            {
                get { return Properties.Resources.UpdatingFunctions; }
            }

            public void PerformTaskOnBackgroundThread()
            {
                Tawala.Common.FunctionRepositoryInfo.Build();
            }

            public void TaskCompleted()
            {
            }

            #endregion
        }

        private class GetThemesTask : IBackgroundTask
        {
            #region IBackgroundTask Members

            public string Description
            {
                get { return Properties.Resources.UpdatingThemes; }
            }

            public void PerformTaskOnBackgroundThread()
            {
                XmlDocument xml = Config.GetProjectThemesFromServer();
                xml.Save(Path.Combine(Config.LocalApplicationData, "ProjectThemes.xml"));
            }

            public void TaskCompleted()
            {
            }

            #endregion
        }

        private class GetSharedDataSourcesTask : IBackgroundTask
        {

            #region IBackgroundTask Members

            public string Description
            {
                get { return Properties.Resources.UpdatingSharedDataSources; }
            }

            public void PerformTaskOnBackgroundThread()
            {
                FieldProviders.QueryServerAndSaveToFile(GlobalSettings.CredentialsElement());
            }

            public void TaskCompleted()
            {
                FieldProviders.LoadDataSourcesFromFile();
                Project.Events.RaiseFieldProvidersChangedEvent(EventArgs.Empty);
            }

            #endregion
        }
    }
}
