// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Common;
using Tawala.DesignerUI.Properties;
using Tawala.Documents;
using Tawala.Forms;
using Tawala.Processes;
using Tawala.Projects;
using Tawala.Projects.Components;
using Tawala.Projects.Documents;
using Tawala.ProjectUI;
using Form=System.Windows.Forms.Form;
using Process=Tawala.Projects.Processes.Process;

namespace Tawala.DesignerUI
{
    /// <summary>
    /// The main form for the Project Designer
    /// </summary>
    public partial class DesignerView : Form, IDesignerView
    {
        private readonly IDesignerPresenter presenter;
        private MdiClient mdiClient;

        public DesignerView()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            presenter = DesignerPresenter.Create(this);

            presenter.Initialize();

            Application.Idle += application_Idle;
        }

        #region IDesignerView

        private bool applicationUIisEnabled = true;
        public IDesignerPresenter Presenter { get { return presenter; } }

        public void SetUIEnableState(bool enable)
        {
            applicationUIisEnabled = enable;

            foreach (Control c in Controls)
            {
                if (c != statusStrip)
                {
                    c.Enabled = enable;
                }
            }
        }

        public void ShowWaitCursor(bool show)
        {
            Cursor = show ? Cursors.WaitCursor : Cursors.Default;
        }

        public void ShowProjectPane(bool show)
        {
            projectPane.Visible = show;
        }

        public void ComponentRemoved(IProjectComponent c)
        {
            foreach (Form f in MdiChildren)
            {
                if (f.Tag == c)
                {
                    killMdiChild(f);
                    break; // should only have one view of a component
                }
            }
        }

        public void CurrentComponentSet(IProjectComponent component)
        {
            // Search for an MDI child form whose Tag matches e.Component
            // If found make sure the window is visible and return.

            foreach (Form child in MdiChildren)
            {
                if (child.Tag == component)
                {
                    // Make the mdi child the active one, which brings it
                    // to the front and (restores it if it was minimized?)
                    child.Activate();
                    return;
                }
            }

            // No MDI child form exists so create a MDI child form for the component
            // Call each create method -- of each type, it will only 
            // change the value of "mdiForm" if it was able to create an MDI child form for the component type.

            try
            {
                Cursor = Cursors.WaitCursor;

                SuspendLayout();

                MDIComponentView mdiForm = null;
                if (component is Projects.Form)
                {
                    MDIFormView.Create(ref mdiForm, component);
                }
                else if (component is Document)
                {
                    MdiDocumentView.Create(ref mdiForm, component);
                }
                else if (component is Process)
                {
                    MDIForm.Create(ref mdiForm, component);
                }

                if (mdiForm != null)
                {
                    mdiForm.MdiParent = this;
                    mdiForm.StartPosition = FormStartPosition.WindowsDefaultLocation;
                    mdiForm.Show();
                }

                ResumeLayout(true);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        public void DestroyMdiChildren()
        {
            foreach (Form f in MdiChildren)
            {
                killMdiChild(f);
            }
        }

        #endregion

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            Project.Events.ComponentRemoving += events_ComponentRemoving;
        }

        private void events_ComponentRemoving(object sender, ComponentCancelEventArgs e)
        {
            if (!ConfirmDialog.ConfirmDelete(e.Component.UserVisibleComponentTypeName, e.Component.Name))
            {
                e.Canceled = true;
            }
        }

        protected override void OnMdiChildActivate(EventArgs e)
        {
            base.OnMdiChildActivate(e);

            var mdiForm = ActiveMdiChild as MDIComponentView;

            if (mdiForm == null || mdiForm.Tag == null)
            {
                Project.Current.SetCurrentComponent(null);
                setPalette(null); // This is for old palette
                return;
            }

            if (mdiForm.Tag != Project.Current.CurrentComponent)
            {
                Project.Current.SetCurrentComponent(ActiveMdiChild.Tag as Component);
            }

            setPalette(mdiForm.Palette); // This is for old palette
        }

        private ComponentPalette getPalette()
        {
            foreach (Control c in Controls)
            {
                if (c is ComponentPalette)
                {
                    return c as ComponentPalette;
                }
            }

            return null;
        }

        private void setPalette(ComponentPalette c)
        {
            SuspendLayout();

            Control curPalette = getPalette();
            if (c != curPalette)
            {
                if (curPalette != null)
                {
                    Controls.Remove(curPalette);
                }

                if (c != null)
                {
                    int index = Controls.GetChildIndex(projectPane);
                    if (projectPane.Visible)
                    {
                        c.Left = projectPane.Left + projectPane.Width;
                        c.Top = projectPane.Top;
                    }
                    else
                    {
                        c.Left = 0;
                        c.Top = projectPane.Top;
                    }
                    Controls.Add(c);
                    Controls.SetChildIndex(c, index);
                }
            }

            ResumeLayout(true);
        }

        private void killMdiChild(Form f)
        {
            f.Hide();
            f.MdiParent = null;
            f.Close();
            f.Dispose();
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            fileDeployToolStripMenuItem.Enabled = projectIsDeployable();
        }

        private void toolStripMenuItemHeader_Click(object sender, EventArgs e)
        {
            var headerDialog = new PageHeaderDialog();
            if (headerDialog.ShowDialog(this) == DialogResult.OK)
            {
            }
        }

        #region Application Events

        private static bool networkIsUp = true;

        /// <summary>
        /// Called when the Application become idle.
        /// Used to update UI state for things that are always visible like ToolBar buttons.
        /// </summary>
        private void application_Idle(object sender, EventArgs e)
        {
            // if menuItemEdit's Tag property is null its not displaying
            // we don't want to do an idle update of the edit menu items while
            // the edit menu is displaying!

            if (menuItemEdit.Tag == null)
            {
                IEditMenu target = getIEditMenu(this);

                bool bCanCut = false;
                bool bCanCopy = false;
                bool bCanPaste = false;
                bool bCanDelete = false;
                bool bCanRename = false;
                bool bCanUndo = false;
                bool bCanRedo = false;

                if (target != null)
                {
                    bCanCut = target.CanCut();
                    bCanCopy = target.CanCopy();
                    bCanPaste = target.CanPaste();
                    bCanDelete = target.CanDelete();
                    bCanRename = target.CanRename();
                    bCanUndo = target.CanUndo();
                    bCanRedo = target.CanRedo();

                    toolStripButtonUndo.ToolTipText = ("Undo " + target.UndoActionText).Trim();
                    toolStripButtonRedo.ToolTipText = ("Redo " + target.RedoActionText).Trim();
                }

                menuItemEditCut.Enabled = tbbCut.Enabled = bCanCut;
                menuItemEditCopy.Enabled = tbbCopy.Enabled = bCanCopy;
                menuItemEditPaste.Enabled = tbbPaste.Enabled = bCanPaste;
                menuItemEditDelete.Enabled = tbbDelete.Enabled = bCanDelete;
                menuItemEditRename.Visible = menuItemEditRename.Enabled = bCanRename;

                menuItemEditUndo.Enabled = toolStripButtonUndo.Enabled = bCanUndo;
                menuItemEditRedo.Enabled = toolStripButtonRedo.Enabled = bCanRedo;
            }

            tbbDeployProject.Enabled = projectIsDeployable();

            var componentView = ActiveMdiChild as MDIComponentView;
            filePrintPreviewToolStripMenuItem.Enabled = componentView != null ? componentView.CanPrint : false;
            filePrintToolStripMenuItem.Enabled = filePrintPreviewToolStripMenuItem.Enabled;

            disableUIforModelessDialog();
        }

        private void disableUIforModelessDialog()
        {
            bool enable = noModelessDialogsActive();

            if (menuStrip.Enabled != enable && applicationUIisEnabled)
            {
                menuStrip.Enabled = enable;
                mainToolStrip.Enabled = enable;

                //projectPane.Enabled = enable;

                //foreach (Form mdiForm in MdiChildren)
                //{
                //    mdiForm.Enabled = enable;
                //}

                ControlBox = enable;
            }
        }

        private bool noModelessDialogsActive()
        {
            return OwnedForms.Length == 0;
        }

        private static bool projectIsDeployable()
        {
            return Project.Current.FormList.Count > 0 && networkIsUp;
        }

        #endregion

        #region Form Events

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            findMDIClientControl();

            presenter.StartInitializationTasks(initializationTasksComplete, initializationTasksProgress);
        }

        private void initializationTasksComplete(bool success)
        {
            statusStrip.Items.Remove(toolStripProgressBarFunctions);

            toolStripStatusLabelFunctions.ForeColor = Color.Black;

            if (success)
            {
                statusStrip.Items.Remove(toolStripStatusLabelFunctions);
            }
            else
            {
                statusStrip.Items.Remove(toolStripProgressBarFunctions);

                toolStripStatusLabelFunctions.Text += " Failed.";
                toolStripStatusLabelFunctions.ForeColor = Color.Red;

                MessageBox.Show(this, Resources.InitializationTasksError, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void initializationTasksProgress()
        {
            int progressStep = toolStripProgressBarFunctions.Maximum/5;

            if (toolStripProgressBarFunctions.Value + progressStep <= toolStripProgressBarFunctions.Maximum)
            {
                toolStripProgressBarFunctions.Value += progressStep;
            }
            else
            {
                toolStripProgressBarFunctions.Value = toolStripProgressBarFunctions.Minimum;
            }
        }

        private void findMDIClientControl()
        {
            // Find the MDIClient control and stache it in mdiClient variable
            foreach (Control c in Controls)
            {
                if (c is MdiClient)
                {
                    mdiClient = c as MdiClient;
                    break;
                }
            }
            Debug.Assert(mdiClient != null && IsMdiContainer);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Side Note:  according to docs most UI events with a cancel property default to false 
            // but apparently that isn't always the case.

            e.Cancel = presenter.SaveProjectIfModified() == DialogResult.Cancel;
        }

        #endregion

        #region File Menu Events

        /// <summary>
        /// When the menuitem (mi) File | Exit is clicked, Close the Designer Windows Form
        /// </summary>
        private void fileExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// When the menuitem (mi) File | New Project is clicked, start a new project
        /// </summary>
        private void fileNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            presenter.NewProject();
        }

        /// <summary>
        /// Handler for Deploy button and menu item
        /// </summary>
        private void fileDeployToolStripMenuItem_Click(object sender, EventArgs e)
        {
            presenter.DeployAsync();
        }

        /// <summary>
        /// Add a new process to the project.
        /// </summary>
        private void fileAddNewProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.Current.AddProcess();
        }

        /// <summary>
        /// Add a new form to the project.
        /// </summary>
        private void fileAddNewFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.Current.AddForm();
        }

        /// <summary>
        /// Add a new document to the project.
        /// </summary>
        private void fileAddNewDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Project.Current.AddDocument();
        }

        private void fileOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            presenter.OpenProject();
        }

        /// <summary>
        /// File Save clicked
        /// </summary>
        private void fileSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            presenter.SaveProject(false);
        }

        /// <summary>
        /// File Save As clicked
        /// </summary>
        private void fileSaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            presenter.SaveProject(true);
        }

        private void filePrintPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var componentView = ActiveMdiChild as MDIComponentView;
            if (componentView != null)
            {
                componentView.PrintPreview();
            }
        }

        private void filePrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var componentView = ActiveMdiChild as MDIComponentView;
            if (componentView != null)
            {
                componentView.Print();
            }
        }

        #endregion

        #region Edit Menu Events

        private void menuItemEdit_DropDownOpening(object sender, EventArgs e)
        {
            menuItemEdit.Tag = "DroppedDown"; // internal, don't localize

            IEditMenu target = getIEditMenu(this);

            if (target != null)
            {
                menuItemEditCut.Enabled = target.CanCut();
                menuItemEditCopy.Enabled = target.CanCopy();
                menuItemEditPaste.Enabled = target.CanPaste();
                menuItemEditDelete.Enabled = target.CanDelete();
                menuItemEditRename.Visible = menuItemEditRename.Enabled = target.CanRename();

                menuItemEditUndo.Enabled = target.CanUndo();
                menuItemEditUndo.Text = "Undo " + target.UndoActionText;
                menuItemEditRedo.Enabled = target.CanRedo();
                menuItemEditRedo.Text = "Redo " + target.RedoActionText;

                ToolStripMenuItem[] add = target.GetAdditionalMenuItems();

                int realItemsAdded = 0; // don't count separators

                if (add != null && add.Length != 0)
                {
                    // NOTE:  ToolStripManager.Merge doesn't work the way I want
                    // Would like to be able to merge in items from a ContextMenuStrip
                    // For right now, the following only works with the ProjectPane
                    // which duplicates the appropriate menu items, sets their state and returns them
                    // in an array.
                    for (int i = 0; i < add.Length; ++i)
                    {
                        if (add[i] != null)
                        {
                            // if not null its a menu item so add it
                            menuItemEdit.DropDownItems.Add(add[i]);
                            realItemsAdded++;
                        }
                        else
                        {
                            // otherwise its a separator for yet another group of menu items so create it
                            menuItemEdit.DropDownItems.Add(new ToolStripSeparator());
                        }
                    }
                }

                // additional menu items added after standard so make sure standard separator is visible
                menuItemEditSeparator.Visible = realItemsAdded != 0;
            }
            else
            {
                menuItemEditCut.Enabled = false;
                menuItemEditCopy.Enabled = false;
                menuItemEditPaste.Enabled = false;
                menuItemEditDelete.Enabled = false;
                menuItemEditRename.Visible = menuItemEditRename.Enabled = false;
                menuItemEditUndo.Enabled = false;
                menuItemEditRedo.Enabled = false;
                menuItemEditSeparator.Visible = false;
            }
        }

        private void menuItemEdit_DropDownClosed(object sender, EventArgs e)
        {
            // Remove anything from edit menu beyond separator

            while (menuItemEdit.DropDownItems.Count > 8)
            {
                menuItemEdit.DropDownItems.RemoveAt(menuItemEdit.DropDownItems.Count - 1);
            }

            menuItemEdit.Tag = null;
        }

        private void menuItemEditCut_Click(object sender, EventArgs e)
        {
            IEditMenu target = getIEditMenu(this);

            if (target != null && target.CanCut())
            {
                target.Cut();
            }
        }

        private void menuItemEditCopy_Click(object sender, EventArgs e)
        {
            IEditMenu target = getIEditMenu(this);

            if (target != null && target.CanCopy())
            {
                target.Copy();
            }
        }

        private void menuItemEditPaste_Click(object sender, EventArgs e)
        {
            IEditMenu target = getIEditMenu(this);

            if (target != null && target.CanPaste())
            {
                target.Paste();
            }
        }

        private void menuItemEditDelete_Click(object sender, EventArgs e)
        {
            IEditMenu target = getIEditMenu(this);

            if (target != null && target.CanDelete())
            {
                target.Delete();
            }
        }

        private void menuItemEditRename_Click(object sender, EventArgs e)
        {
            IEditMenu target = getIEditMenu(this);

            if (target != null && target.CanRename())
            {
                target.Rename();
            }
        }

        private void menuItemEditUndo_Click(object sender, EventArgs e)
        {
            IEditMenu target = getIEditMenu(this);

            if (target != null && target.CanUndo())
            {
                target.Undo();
            }
        }

        private void menuItemEditRedo_Click(object sender, EventArgs e)
        {
            IEditMenu target = getIEditMenu(this);

            if (target != null && target.CanRedo())
            {
                target.Redo();
            }
        }

        private IEditMenu getIEditMenu(Control parent)
        {
            IEditMenu target = null;

            foreach (Control c in parent.Controls)
            {
                if (c.ContainsFocus)
                {
                    if (c is IEditMenu)
                    {
                        target = c as IEditMenu;
                    }

                    IEditMenu innerTarget = getIEditMenu(c);

                    if (innerTarget != null)
                    {
                        target = innerTarget;
                    }

                    break;
                }
            }
            return target;
        }

        #endregion

        #region View Menu Events

        private void toolStripMenuView_DropDownOpening(object sender, EventArgs e)
        {
            toolStripMenuViewProjectPane.Checked = projectPane.Visible;
            toolStripMenuViewFieldsPalette.Checked = FieldsPalette.Palette.Visible;
        }

        private void toolStripMenuViewProjectPane_Click(object sender, EventArgs e)
        {
            projectPane.Visible = toolStripMenuViewProjectPane.Checked = !projectPane.Visible;
        }

        private void toolStripMenuViewFieldsPalette_Click(object sender, EventArgs e)
        {
            toolStripMenuViewFieldsPalette.Checked = FieldsPalette.Palette.Visible = !FieldsPalette.Palette.Visible;
        }

        private void menuItemViewToolbar_Click(object sender, EventArgs e)
        {
            mainToolStrip.Visible = menuItemViewToolbar.Checked = !menuItemViewToolbar.Checked;
        }

        private void menuItemViewStatusBar_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = menuItemViewStatusBar.Checked = !menuItemViewStatusBar.Checked;
        }

        #endregion

        #region Tools Menu Events

        private void toolStripMenuItemProjectManager_Click(object sender, EventArgs e)
        {
            presenter.LaunchProjectManager();
        }

        #endregion

        #region Format Menu Events

        private void menuItemFormat_DropDownOpening(object sender, EventArgs e)
        {
            SortedDictionary<string, string> themes = Config.GetProjectThemeList();

            projectThemesToolStripMenuItem.DropDownItems.Clear();
            projectThemesToolStripMenuItem.Enabled = themes.Count > 0;

            foreach (string name in themes.Keys)
            {
                string path = themes[name];
                var theme = new ToolStripMenuItem(name);
                theme.Checked = Project.Current.ThemePath.CompareTo(path) == 0;
                theme.CheckOnClick = true;
                theme.CheckedChanged += theme_CheckedChanged;
                theme.Tag = path;
                projectThemesToolStripMenuItem.DropDownItems.Add(theme);
            }
        }

        private void theme_CheckedChanged(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Checked)
            {
                Project.Current.ThemePath = (((ToolStripMenuItem)sender).Tag) as string;
            }
        }

        #endregion

        #region Help Menu Events

        private void menuItemHelpAbout_Click(object sender, EventArgs e)
        {
            new About().ShowDialog(this);
        }

        #endregion

        #region Window Menu Events

        private void toolStripMenuItemWindows_DropDownOpening(object sender, EventArgs e)
        {
            bool bEnable = MdiChildren.Length > 0;
            windowsCascadeToolStripMenuItem.Enabled = bEnable;
            windowsTileHorzToolStripMenuItem.Enabled = bEnable;
            windowsTileVertToolStripMenuItem.Enabled = bEnable;
            windowsCloseAllToolStripMenuItem.Enabled = bEnable;

            windows1ToolStripSeparator.Visible = bEnable;
        }

        private void windowsCascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void windowsTileHorzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void windowsTileVertToolStripMenuItem_Click_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void windowsCloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DestroyMdiChildren();
        }

        #endregion
    }
}