// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Tawala.Common;
using Tawala.DesignerUI.Properties;
using Tawala.Projects;
using Tawala.Projects.Exceptions;
using Tawala.ProjectUI;
using Process=System.Diagnostics.Process;

namespace Tawala.DesignerUI
{
    public class DesignerPresenter : IDesignerPresenter
    {
        private BackgroundTaskQueue backgroundTasksQueue;
        private string captionPrefix = string.Empty;

        private BackgroundTaskQueue.Task[] initializationTasks =
            {
                new BuildFunctionsTask(),
                new GetFieldProvidersTask(),
                new CheckForUpdateTask(),
                new GetProjectThemesTask()
            };

        private string projectDefaultDirectory = Config.DefaultProjectDirectory;
        private string projectFullPath;

        private bool projectOpenInProgress;
        private IDesignerView view;

        #region Project Events

        private void project_NewProject(object sender, ProjectEventArgs e)
        {
            view.DestroyMdiChildren();
            projectFullPath = null;
            UpdateCaption();
            view.ShowProjectPane(true);

            UpdateDeploymentInfoAsync();
        }

        private void project_OpeningProject(object sender, EventArgs e)
        {
            UpdateFieldProviderInfo();
        }

        private void project_ProjectOpened(object sender, ProjectEventArgs e)
        {
            view.DestroyMdiChildren();
            view.ShowProjectPane(true);

            UpdateDeploymentInfoAsync();
        }

        private void project_ComponentAdded(object sender, ComponentEventArgs e)
        {
            if (!projectOpenInProgress)
            {
                Project.Current.SetCurrentComponent(e.Component);
            }
        }

        private void project_ComponentRemoved(object sender, ComponentEventArgs e)
        {
            view.ComponentRemoved(e.Component);
        }

        private void project_CurrentComponentSet(object sender, ComponentEventArgs e)
        {
            if (e.Component == null || projectOpenInProgress)
            {
                return;
            }

            view.CurrentComponentSet(e.Component);
        }

        #endregion

        private DesignerPresenter(IDesignerView view)
        {
            this.view = view;
            backgroundTasksQueue = new BackgroundTaskQueue(view);
        }

        #region IDesignerPresenter Members

        public void Initialize()
        {
            setupCurrentDirectory();
            initializeCaption();

            Project.Events.ProjectOpened += project_ProjectOpened;
            Project.Events.ComponentAdded += project_ComponentAdded;
            Project.Events.ComponentRemoved += project_ComponentRemoved;
            Project.Events.CurrentComponentSet += project_CurrentComponentSet;

            Project.New();
            UpdateCaption();

            UpdateDeploymentInfoAsync();

            Project.Events.NewProject += project_NewProject;
        }

        public string ProjectFullPath { get { return projectFullPath; } }

        public string ProjectDefaultDirectory { get { return projectDefaultDirectory; } }

        public void UpdateCaption()
        {
            view.Text = captionPrefix + Project.Current.Name;
        }

        public void LaunchProjectManager()
        {
            var psi = new ProcessStartInfo();
            psi.FileName = Config.ProjectManagerURL;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(psi);
        }

        public void UpdateFieldProviderInfo()
        {
            backgroundTasksQueue.Add(new GetFieldProvidersTask());
            Application.DoEvents();

            for (int i = 0; i < 60; i++)
            {
                if (backgroundTasksQueue.IsEmpty(500))
                {
                    return;
                }

                Application.DoEvents();
            }
        }

        public void StartInitializationTasks(Action<bool> finished, MethodInvoker progress)
        {
            backgroundTasksQueue.Add(initializationTasks);
            var poller = new InitializationTaskPoller(this);
            poller.Start(finished, progress);
        }

        public bool InitializationTasksCompleted()
        {
            return !backgroundTasksQueue.IsBusy;
        }

        public void DeployAsync()
        {
            // Force the project to be saved. This will also force it to be named if it hasn't been named yet.
            if (!SaveProject(false))
            {
                return; // bail as user aborted save
            }

            new DeployingProjectForm(backgroundTasksQueue).ShowDialog(view);
        }

        public void UpdateDeploymentInfoAsync()
        {
            backgroundTasksQueue.Add(new GetDeploymentInfoTask());
        }

        public bool IsBackgroundTaskQueueBusy { get { return backgroundTasksQueue.IsBusy; } }

        public bool ValidateProject(string path)
        {
            var validator = new TawalaProjectValidator();
            if (validator.ValidateXML())
            {
                return true;
            }

            string root = path;
            string extension = "." + Resources.ProjectExt;

            int i = path.LastIndexOf(extension);
            if (i > 0)
            {
                root = path.Remove(i);
            }
            // use this extension for bad because of the way Save affects the project name
            // it strips off the last extension so if we had .tawala.bad we would
            // end up with a project name of "name.tawala" in the caption

            string time = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            //string secondPassXmlFilePath = root + extension + "-pass2-" + time;
            //File.WriteAllText(secondPassXmlFilePath, validator.Message);

            string debugFilePath = root + extension + "-debug-" + time;
            Project.SaveWithDebugInfo(debugFilePath, validator.Message);

            var dlg = new SavedProjectInvalid(debugFilePath);
            dlg.ShowDialog(view);

            return false;
        }

        public void NewProject()
        {
            if (SaveProjectIfModified() == DialogResult.Cancel)
            {
                return;
            }

            Globals.CloseAllOpenDialogs();

            // not true for Win2K so the result will be a new empty project with no dialog on Win2K.
            if (OSFeature.Feature.IsPresent(OSFeature.Themes))
            {
                var dialog = new NewProjectDialog(Path.Combine(Config.AppDirectory, "Templates"));
                if (dialog.ShowDialog(view) == DialogResult.OK)
                {
                    if (dialog.SelectedTemplateFile != null)
                    {
                        openProject(dialog.SelectedTemplateFile, true);
                        if (dialog.EmptyTemplateSelected)
                        {
                            Project.Current.ResetModifiedState();
                        }
                    }
                }
                return;
            }

            Project.New();
            UpdateFieldProviderInfo();
        }

        public void OpenProject()
        {
            if (SaveProjectIfModified() == DialogResult.Cancel)
            {
                return;
            }

            Globals.CloseAllOpenDialogs();

            var ofd = new OpenFileDialog();
            ofd.InitialDirectory = projectDefaultDirectory;
            ofd.Filter = Resources.OpenProjectFilter;
            ofd.DefaultExt = Resources.ProjectExt;

            string openFileName = null;

            if (ofd.ShowDialog(view) == DialogResult.OK)
            {
                openFileName = ofd.FileName;
            }

            ofd = null;

            openProject(openFileName, false);
        }

        public bool SaveProject(bool saveAs)
        {
            string tempFullPath = projectFullPath;

            if (tempFullPath == null)
            {
                saveAs = true;
            }
            else
            {
                // prevent unintentional saving of project file with XML extension
                if (Path.GetExtension(tempFullPath) == ("." + Resources.ProjectXmlExt))
                {
                    // replace .xml with .tawala and force file dialog to appear
                    tempFullPath = Path.GetFileNameWithoutExtension(tempFullPath);
                    tempFullPath += "." + Resources.ProjectExt;
                    saveAs = true;
                }
            }

            if (saveAs)
            {
                var sfd = new SaveFileDialog();
                sfd.InitialDirectory = projectDefaultDirectory;
                sfd.FileName = tempFullPath != null ? tempFullPath : Project.Current.Name;
                sfd.Filter = Resources.SaveProjectFilter;
                sfd.DefaultExt = Resources.ProjectExt;

                if (sfd.ShowDialog(view) != DialogResult.OK)
                {
                    return false;
                }

                projectFullPath = sfd.FileName;
                projectDefaultDirectory = Path.GetDirectoryName(sfd.FileName);
                Project.Current.Name = Path.GetFileNameWithoutExtension(projectFullPath);
            }

            view.ShowWaitCursor(true);

            try
            {
                if (ValidateProject(projectFullPath))
                {
                    Project.Save(projectFullPath);
                    UpdateCaption();
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Resources.SaveProjectFailureCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                view.ShowWaitCursor(false);
            }

            return false;
        }

        public DialogResult SaveProjectIfModified()
        {
            DialogResult result = DialogResult.No; // if project not modifed DialogResult.No is returned

            if (Project.Current.Modified)
            {
                result = MessageBox.Show(view, Resources.SaveOnExitNewLoadMessage, Application.ProductName,
                                         MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (result == DialogResult.Yes)
                {
                    if (!SaveProject(false))
                    {
                        return DialogResult.Cancel;
                    }
                }
            }

            return result;
        }

        #endregion

        public static IDesignerPresenter Create(IDesignerView view)
        {
            return new DesignerPresenter(view);
        }

        private void openProject(string openFileName, bool isTemplate)
        {
            if (!string.IsNullOrEmpty(openFileName))
            {
                view.ShowWaitCursor(true);
                bool fieldsPaletteVisibility = FieldsPalette.Palette.Visible;

                try
                {
                    projectOpenInProgress = true;

                    FieldsPalette.Palette.Visible = false;

                    Project.New();

                    if (!openProjectFile(openFileName))
                    {
                        return;
                    }

                    if (isTemplate)
                    {
                        projectFullPath = null;
                        projectDefaultDirectory = Config.DefaultProjectDirectory;
                        Project.Current.Name = "Untitled";
                    }
                    else
                    {
                        projectFullPath = openFileName;
                        projectDefaultDirectory = Path.GetDirectoryName(openFileName);
                        Project.Current.Name = Path.GetFileNameWithoutExtension(projectFullPath);
                    }

                    UpdateCaption();
                }
                catch (ProjectMissingDataSourcesException exception)
                {
                    fieldsPaletteVisibility = false;
                    view.ShowWaitCursor(false);
                    Project.New();
                    new DataSourcesMissing(exception.MissingDataSourceNames).ShowDialog(view);
                }
                finally
                {
                    view.ShowWaitCursor(false);
                    projectOpenInProgress = false;
                    FieldsPalette.Palette.Visible = fieldsPaletteVisibility;
                }
            }
        }

        private static bool openProjectFile(string openFileName)
        {
            Project.ProjectFileOpenResult result = Project.Open(openFileName);

            if (result != Project.ProjectFileOpenResult.OK)
            {
                if (result == Project.ProjectFileOpenResult.InvalidFile)
                {
                    MessageBox.Show(Resources.InvalidFileError, Resources.ErrorCaption, MessageBoxButtons.OK);
                }
                if (result == Project.ProjectFileOpenResult.NewerXmlFormat)
                {
                    MessageBox.Show(Resources.NewerXmlFormatVersion, Resources.ErrorCaption, MessageBoxButtons.OK);
                }
                return false;
            }
            return true;
        }

        private void initializeCaption()
        {
            string text = view.Text;

            // Put the build # in the title if iteration build
            // If no config.xml in app directory build # will be 0.
            if (Config.RuntimeEnvironment == Config.RuntimeType.Dev)
            {
                string build = " #" + Config.Build + " (DEV) - ";
                text = text.Replace(" -", build);
            }
            else if (Config.RuntimeEnvironment == Config.RuntimeType.Build)
            {
                string build = " #" + Config.Build + " (BUILD) - ";
                text = text.Replace(" -", build);
            }

            view.Text = text;
            captionPrefix = text;
        }

        private void setupCurrentDirectory()
        {
            Assembly entry = Assembly.GetEntryAssembly();
            string location = entry.Location;
            string directory = Path.GetDirectoryName(location);
            Directory.SetCurrentDirectory(directory);
        }

        private void handleProjectOnCommandLine()
        {
            string dotExt = "." + Resources.ProjectExt;
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg.EndsWith(dotExt, StringComparison.InvariantCultureIgnoreCase) && projectFullPath == null)
                {
                    if (!openProjectFile(arg))
                    {
                        return;
                    }

                    projectFullPath = arg;
                    projectDefaultDirectory = Path.GetDirectoryName(arg);

                    Project.Current.Name = Path.GetFileNameWithoutExtension(arg);
                    UpdateCaption();
                }
            }
        }

        #region Nested type: InitializationTaskPoller

        private class InitializationTaskPoller : Timer
        {
            private Action<bool> finishedCallback;
            private DesignerPresenter presenter;
            private MethodInvoker progressCallback;

            public InitializationTaskPoller(DesignerPresenter dpresenter)
            {
                presenter = dpresenter;
            }

            public void Start(Action<bool> onfinished, MethodInvoker progress)
            {
                finishedCallback = onfinished;
                progressCallback = progress;

                presenter.view.SetUIEnableState(false);

                Interval = 500;
                Start();
            }

            protected override void OnTick(EventArgs e)
            {
                if (presenter.InitializationTasksCompleted())
                {
                    Stop();

                    if (FunctionRepositoryInfo.IsLoaded)
                    {
                        presenter.view.SetUIEnableState(true);
                        finishedCallback(true);
                        presenter.handleProjectOnCommandLine();
                        Project.Events.OpeningProject += presenter.project_OpeningProject;
                    }
                    else
                    {
                        finishedCallback(false);
                    }

                    cleanup();
                }
                else
                {
                    progressCallback();
                }
            }

            private void cleanup()
            {
                Dispose();

                finishedCallback = null;
                progressCallback = null;
                presenter = null;
            }
        }

        #endregion
    }
}