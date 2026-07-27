// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.IO;
using Tawala.Common;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Deployment;
using Tawala.FontSupport;
using Tawala.Interfaces;
using Tawala.MainApplication;
using TawalaDesigner.Dialogs;

namespace TawalaDesigner
{
    public partial class ApplicationView : System.Windows.Forms.Form, IApplicationView
	{
		public ApplicationView()
		{
			InitializeComponent();

			ApplicationPresenter.View = this;

			setupFontFamilyList();
		}

		private static void setupFontFamilyList()
		{
			if (privateFontCollection == null)
			{
				privateFontCollection = new PrivateFontCollection();
				privateFontCollection.AddFontFile(Fonts.DefaultFontFilename);
			}

			var familyList = new List<FontFamily>();

			familyList.Add(privateFontCollection.Families[0]);

			InstalledFontCollection ifc = new InstalledFontCollection();

			foreach (var f in ifc.Families)
			{
				if (Fonts.WebSafeFonts.Contains(f.Name))
				{
					familyList.Add(f);
				}
			}

			fontFamilies = familyList.ToArray();
		}

		protected override void OnLoad(EventArgs e)
		{

            Width = 1024;
			Height = 768;

            Text = Properties.Resources.ApplicationTitlePrefix + buildName();

            base.OnLoad(e);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            Application.DoEvents();

            Project.New();

            new StartupTasksView().ShowDialog(this);

            Application.Idle += application_Idle;
        }

		private static PrivateFontCollection privateFontCollection;
		private static FontFamily[] fontFamilies;

		private IFormView formView = null;

		protected override void OnMdiChildActivate(EventArgs e)
		{
			formView = null;

			base.OnMdiChildActivate(e);

			formView = ActiveMdiChild as IFormView;
		}

		#region File Menu Events

		private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (projectExplorerView.Presenter.ProjectCloseRequested())
			{
				NewProjectDialog dialog = new NewProjectDialog(Path.Combine(Config.AppDirectory, "Templates"));

				if (dialog.ShowDialog(this) == DialogResult.OK)
				{
					if (dialog.SelectedTemplateFile != null)
					{
						projectExplorerView.Presenter.NewProjectRequested();
						projectExplorerView.OpenProjectTemplateFile(dialog.SelectedTemplateFile);
					}
				}
			}
		}

		private static string projectFolder = Config.DefaultProjectDirectory;
		private static readonly string defaultExt = ".xml";
		private static readonly string fileExtFilter = "Form UI Test Files (*.xml)|*.xml";

		private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = createOpenFileDialog();

			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				projectExplorerView.Presenter.ProjectOpenRequested(openFileDialog.FileName);
			}
		}

		private OpenFileDialog createOpenFileDialog()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = projectExplorerView.Presenter.ProjectFolder;
			openFileDialog.Filter = Properties.Resources.OpenProjectFilter;
			openFileDialog.DefaultExt = Properties.Resources.ProjectExt;

			return openFileDialog;
		}

		private void openProject()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.InitialDirectory = projectFolder;
			ofd.Filter = fileExtFilter;
			ofd.DefaultExt = defaultExt;

			string projectFilePath = null;

			if (ofd.ShowDialog(this) == DialogResult.OK)
			{
				projectFilePath = ofd.FileName;
			}

			ofd = null;

			projectExplorerView.OpenProjectFile(projectFilePath);
		}

		private void closeExistingFormViews()
		{
			foreach (System.Windows.Forms.Form form in MdiChildren)
			{
				form.Close();
			}
		}

		private void addNewProcessToolStripMenuItem_Click(object sender, EventArgs e)
		{
			projectExplorerView.Presenter.NewProcessRequested();
		}

		private void addNewFormToolStripMenuItem_Click(object sender, EventArgs e)
		{
			projectExplorerView.Presenter.NewFormRequested();
		}

		private void addNewDocumentToolStripMenuItem_Click(object sender, EventArgs e)
		{
			projectExplorerView.Presenter.NewDocumentRequested();
		}

		private void saveProjectToolStripMenuItem_Click(object sender, EventArgs args)
		{
			if (projectIsUnsaved())
			{
				SaveFileDialog saveFileDialog = CreateSaveFileDialog();

				if (saveFileDialog.ShowDialog(formView) == DialogResult.OK)
				{
					projectExplorerView.Presenter.ProjectSaveRequested(saveFileDialog.FileName);
				}
			}
			else
			{
				projectExplorerView.Presenter.ProjectSaveRequested(projectExplorerView.Presenter.ProjectFilePath);
			}
		}

		private bool projectIsUnsaved()
		{
			return projectExplorerView.Presenter.ProjectFilePath == null;
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var saveFileDialog = CreateSaveFileDialog();

			if (saveFileDialog.ShowDialog(formView) == DialogResult.OK)
			{
				projectExplorerView.Presenter.ProjectSaveRequested(saveFileDialog.FileName);
			}
		}

		public SaveFileDialog CreateSaveFileDialog()
		{
			var saveFileDialog = new SaveFileDialog();
			saveFileDialog.InitialDirectory = projectExplorerView.Presenter.ProjectFolder;
			saveFileDialog.FileName = projectExplorerView.Presenter.ProjectName;
			saveFileDialog.Filter = Properties.Resources.SaveProjectFilter;
			saveFileDialog.DefaultExt = Properties.Resources.ProjectExt;

			return saveFileDialog;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		#endregion

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		#region IApplicationView Members

		public FontFamily[] GetFontList()
		{
			return fontFamilies;
		}

		public void SetProjectNameInTitleBar(string projectName)
		{
			string caption = Properties.Resources.ApplicationTitlePrefix;

			if (!String.IsNullOrEmpty(projectName))
			{
                caption += " - " + projectName;
			}

		    caption += buildName();

		    Text = caption;
		}

		#endregion

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			projectExplorerView.Presenter.ComponentPasteRequested();
		}

		#region IApplicationView Members


		public Control ComponentPalette
		{
			get { return componentPalette; }
		}

		#endregion

		private void windowToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			bool enable = MdiChildren.Length > 0;
			windowCascadeToolStripMenuItem.Enabled = enable;
			windowTileHorizontalToolStripMenuItem.Enabled = enable;
			windowTileVerticalToolStripMenuItem.Enabled = enable;
			windowCloseAllToolStripMenuItem.Enabled = enable;

			windowMdiChildrenToolStripSeparator.Visible = enable;
		}

		private void windowCascadeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.Cascade);
		}

		private void windowTileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.TileHorizontal);
		}

		private void windowsTileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.TileVertical);
		}

		private void windowCloseAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			closeExistingFormViews();
		}

		private void toolbarToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			mainToolStrip.Visible = toolbarToolStripMenuItem.Checked;
		}

		private void statusbarToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			statusStrip.Visible = statusbarToolStripMenuItem.Checked;
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new HelpAboutView().ShowDialog(this);
		}

		private void editToolStripMenuItem_DropDownOpened(object sender, EventArgs e)
		{
			ApplicationPresenter.RaiseEditDropDownOpenedEvent(sender, new DropDownOpenedEventArgs());
		}

        private void deployToolStripMenuItem_Click(object sender, EventArgs e)
        {
			if (projectIsUnsaved())
			{
				SaveFileDialog saveFileDialog = CreateSaveFileDialog();

				if (saveFileDialog.ShowDialog(formView) == DialogResult.OK)
				{
					projectExplorerView.Presenter.ProjectSaveRequested(saveFileDialog.FileName);
				}
			}

			new DeployingProjectView().ShowDialog(this);
		}

        private void pageHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

		private void application_Idle(object sender, EventArgs e)
		{
			deployToolStripButton.Enabled = deployToolStripMenuItem.Enabled = ApplicationPresenter.ProjectIsDeployable();

			bool canPaste =
				projectExplorerView.Presenter.CanPasteDocument ||
				projectExplorerView.Presenter.CanPasteForm ||
				projectExplorerView.Presenter.CanPasteProcess;

			pasteToolStripMenuItem.Enabled = canPaste;
			toolStripButtonPaste.Enabled = canPaste;
        }

        private void projectToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            SortedDictionary<string, string> themes = Config.GetProjectThemeList();

            projectThemesToolStripMenuItem.DropDownItems.Clear();
            projectThemesToolStripMenuItem.Enabled = themes.Count > 0;

            foreach (string name in themes.Keys)
            {
                string path = themes[name];
                ToolStripMenuItem theme = new ToolStripMenuItem(name);
                theme.Checked = Project.Current.ThemePath.CompareTo(path) == 0;
                theme.CheckOnClick = true;
                theme.CheckedChanged += delegate { if (theme.Checked) Project.Current.ThemePath = path; };
                projectThemesToolStripMenuItem.DropDownItems.Add(theme);
            }
        }

		private void ApplicationView_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = !projectExplorerView.Presenter.ProjectCloseRequested();
		}

        private string buildName()
        {
            if (Config.RuntimeEnvironment == Config.RuntimeType.Production)
            {
                return string.Empty;
            }

            return string.Format("  ({0})", Config.RuntimeEnvironment.ToString().ToUpper());
        }
	}
}
