// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Tawala.Processes.Properties;
using Tawala.Projects;
using Tawala.Projects.Components;
using Tawala.Projects.Documents;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using Form=System.Windows.Forms.Form;

namespace Tawala.Processes
{
    /// <summary>
    /// The view class that encapsulates the entire Process UI which is added to the designer's viewPanel when
    /// a process is being shown.
    /// </summary>
    public partial class MDIForm : MDIComponentView
    {
        public static readonly Type[] StatementViewTypes =
            {
                null,
                typeof(IfStatementView),
                null,
                typeof(ShowStatementView),
                typeof(SendStatementView),
                null,
                typeof(AppendStatementView),
                null,
                typeof(GetStatementView),
                typeof(ForEachStatementView),
                typeof(DeleteStatementView),
                null,
                typeof(SetStatementView),
                null,
                typeof(CommentStatementView),
                null
            };

        private static StatementPalette palette;
        private ViewInfoBar viewInfoBar;

        /// <summary>
        /// This constructor is solely for keeping the VSN Form Designer happy
        /// </summary>
        public MDIForm()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        /// <summary>
        /// This is the constructor that the Designer form (Designer.cs) should use.
        /// </summary>
        public MDIForm(Process process) : base(process)
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            Collection<ToolStripItem> insertStripArray = editor.Init(Palette as StatementPalette, StatementViewTypes);
            foreach (ToolStripItem menu in insertStripArray)
            {
                insertToolStripMenuItem.DropDownItems.Add(menu);
            }

            Icon = Resources.Process;

            // Hook project events

            Project.Events.ComponentRenamed += project_ComponentRenamed;
            Project.Events.ComponentRemoved += project_ComponentRemoved;

            // Initialize map of Statement types to Details objects.  Also set Tag property of corresponding statement
            // button to the type of the statement.

            editor.Process = process;
        }

        public override ComponentPalette Palette
        {
            get
            {
                if (palette == null)
                {
                    palette = new StatementPalette();
                    palette.Dock = DockStyle.Left;
                    palette.Init(StatementViewTypes);
                }

                return palette;
            }
        }

        public override bool CanPrint { get { return true; } }

        public override void PrintPreview()
        {
            PrintDocument printDoc = new ProcessPrintDocument(editor);
            var dlgPrintPreview = new PrintPreviewDialog();

            // Set any optional properties of dlgPrintPreview here...
            dlgPrintPreview.PrintPreviewControl.Zoom = 1.0;
            dlgPrintPreview.Document = printDoc;
            dlgPrintPreview.UseAntiAlias = true;
            dlgPrintPreview.HandleCreated += dlgPrintPreview_HandleCreated;
            dlgPrintPreview.ShowDialog(ParentForm);
        }

        public override void Print()
        {
            PrintDocument printDoc = new ProcessPrintDocument(editor);
            var dlgPrint = new PrintDialog();

            dlgPrint.ShowHelp = true;
            dlgPrint.Document = printDoc;

            if (dlgPrint.ShowDialog(ParentForm) == DialogResult.OK)
            {
                printDoc.Print();
            }
        }

        private void dlgPrintPreview_HandleCreated(object sender, EventArgs e)
        {
            int inset = 32;
            var s = new Size(ParentForm.Size.Width - inset, ParentForm.Size.Height - inset);
            var dlgPrintPreview = sender as Form;
            dlgPrintPreview.Size = s;
            dlgPrintPreview.StartPosition = FormStartPosition.CenterParent;
            dlgPrintPreview.Text = Application.ProductName + " " + dlgPrintPreview.Text;
        }

        public static void Create(ref MDIComponentView f, IProjectComponent component)
        {
            if (f == null && component is Process)
            {
                f = new MDIForm(component as Process);
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            statementsPaletteToolStripMenuItem.Checked = Palette.Visible;

            editor.ConnectProjectEvents(true);
            editor.RefreshLines();

            Project.Events.RaiseProcessChangedEvent(new ProcessEventArgs(Tag as Process, editor.InsertionPoint));
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            editor.ConnectProjectEvents(false);
            Project.Events.RaiseProcessChangedEvent();
        }

        protected override void OnCreateControl()
        {
            viewInfoBar = new ViewInfoBar(editor.Process);
            viewInfoBar.Dock = DockStyle.Top;
            editor.SplitContainer.Panel1.Controls.Add(viewInfoBar);

            base.OnCreateControl();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            editor.ConnectProjectEvents(false);
            base.OnHandleDestroyed(e);
        }

        private void statementsPaletteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Palette.Visible = !Palette.Visible;
            statementsPaletteToolStripMenuItem.Checked = Palette.Visible;
        }

        #region Project Events

        private void project_ComponentRemoved(object sender, ComponentEventArgs e)
        {
            componentChanged(e.Component);
        }

        private void project_ComponentRenamed(object sender, ComponentRenamedEventArgs e)
        {
            componentChanged(e.Component);
        }

        private void componentChanged(IProjectComponent component)
        {
            if ((component is Document || component is IForm) && Tag != null)
            {
                editor.ResetUndo();
                editor.RefreshLines();
            }
        }

        #endregion
    }
}