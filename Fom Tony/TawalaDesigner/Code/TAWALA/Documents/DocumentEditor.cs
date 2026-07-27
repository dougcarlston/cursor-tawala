// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Functions.Controls;
using Tawala.Functions.Runtime;
using Tawala.Functions.ViewPresenter;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Fields;
using Tawala.Projects.Processes;
using Tawala.ProjectUI;
using Tawala.TextEditor;

namespace Tawala.Documents
{
    /// <summary>
    /// Summary description for TextEditor.
    /// </summary>
    public class DocumentEditor : TextEdit, IEditMenu
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components;

        private RtfDocument document;
        private bool modified;

        public DocumentEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // Enable double-buffering for this control as well as redrawing on resizing

            DoubleBuffered = true;
            ResizeRedraw = true;
        }

        #region IEditMenu Members

        /// <summary>
        /// The Designer form looks for this interface to route main menu cut/copy/paste/delete events to the
        /// control with focus.
        /// </summary>
        bool IEditMenu.CanCut()
        {
            return CanCut;
        }

        bool IEditMenu.CanCopy()
        {
            return CanCopy;
        }

        bool IEditMenu.CanDelete()
        {
            return CanCut;
        }

        bool IEditMenu.CanPaste()
        {
            return CanPaste;
        }

        bool IEditMenu.CanRename()
        {
            return false;
        }

        void IEditMenu.Cut()
        {
            if (CanCut)
            {
                Cut();
            }
        }

        void IEditMenu.Copy()
        {
            if (CanCopy)
            {
                Copy();
            }
        }

        void IEditMenu.Delete()
        {
            if (CanCut)
            {
                Delete();
            }
        }

        void IEditMenu.Paste()
        {
            Paste();
            modified = true;
        }

        void IEditMenu.Rename()
        {
        }

        bool IEditMenu.CanUndo()
        {
            return false;
        }

        bool IEditMenu.CanRedo()
        {
            return false;
        }

        void IEditMenu.Undo()
        {
        }

        void IEditMenu.Redo()
        {
        }

        string IEditMenu.UndoActionText { get { return ""; } }

        string IEditMenu.RedoActionText { get { return ""; } }

        ToolStripMenuItem[] IEditMenu.GetAdditionalMenuItems()
        {
            return null;
        }

        #endregion

        private void events_FormItemChanged(object sender, FormItemEventArgs e)
        {
            updateFieldsAndFunctions();
        }

        public void SetStyle(string name)
        {
        }

        public void Save()
        {
            if (modified)
            {
                modified = false;
                document.Rtf = GetRTF();
                document.Text = null;
            }
        }

        public void InsertField(IField field)
        {
            if (field != null)
            {
                AddField(field.Id, (field).QualifiedFieldName);
                Focus();
            }
        }

        private void updateFieldsAndFunctions()
        {
            if (ContainsStandardFields())
            {
                UpdateFieldNames(Project.FieldMapById.GetQualifiedFieldDictionary());
            }

            if (ContainsFunctionFields())
            {
                UpdateFunctionFieldText(Project.FunctionMapById.GetDisplayStringDictionary());
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.A))
            {
                SelectAll();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            HandleDestroyed += documentEditor_HandleDestroyed;
            // this will not be true when within the design view
            if (ParentForm is MdiDocumentView)
            {
                document = (ParentForm).Tag as RtfDocument;
                if (!string.IsNullOrEmpty(document.Rtf))
                {
                    SetRTF(document.Rtf);
                }
                Changed += textEdit_Changed;
                AllowDrop = true;
            }

            attachEvents();
        }

        private void events_LineListChanged(object sender, StatementEventArgs e)
        {
            if (e.Statement is SetStatement || e.Statement is ArithmeticStatement)
            {
                updateFieldsAndFunctions();
            }
        }

        private void events_ComponentRenamed(object sender, ComponentRenamedEventArgs e)
        {
            if (e.Component is IForm)
            {
                updateFieldsAndFunctions();
            }
        }

        private void documentEditor_HandleDestroyed(object sender, EventArgs e)
        {
            detachEvents();
        }

        private void detachEvents()
        {
            Project.Events.FormItemAdded -= events_FormItemChanged;
            Project.Events.FormItemRemoved -= events_FormItemChanged;
            Project.Events.FormItemChanged -= events_FormItemChanged;

            Project.Events.StatementModified -= events_LineListChanged;
            Project.Events.StatementRemoved -= events_LineListChanged;

            Project.Events.ComponentRenamed -= events_ComponentRenamed;
        }

        private void attachEvents()
        {
            Project.Events.FormItemAdded += events_FormItemChanged;
            Project.Events.FormItemRemoved += events_FormItemChanged;
            Project.Events.FormItemChanged += events_FormItemChanged;

            Project.Events.StatementModified += events_LineListChanged;
            Project.Events.StatementRemoved += events_LineListChanged;

            Project.Events.ComponentRenamed += events_ComponentRenamed;
        }

        #region TextEdit Events

        /// <summary>
        /// Every time the RichTextBox text changes, update the document object's text.
        /// </summary>
        private void textEdit_Changed(object sender, EventArgs e)
        {
            modified = true;
        }

        public override void HandleDragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(typeof(IPaletteField)) ||
                e.Data.GetDataPresent(typeof(Variable)))
            {
                e.Effect = CanDrop ? DragDropEffects.Copy : DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        public override void HandleDragOver(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(typeof(IPaletteField)) ||
                e.Data.GetDataPresent(typeof(Variable)))
            {
                e.Effect = CanDrop ? DragDropEffects.Copy : DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        public override void HandleDragDrop(DragEventArgs e)
        {
            if (!CanDrop)
            {
                return;
            }

            var ptScreen = new Point(e.X, e.Y);

            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                Select(ptScreen, 0);
                Selection.Text = (string)e.Data.GetData(DataFormats.Text);
            }
            else if (e.Data.GetDataPresent(typeof(IPaletteField)))
            {
                Select(ptScreen, 0);
                var field = (IPaletteField)e.Data.GetData(typeof(IPaletteField));
                AddField(field.Id, field.QualifiedFieldName);
            }
            else if (e.Data.GetDataPresent(typeof(Variable)))
            {
                Select(ptScreen, 0);
                var field = (IPaletteField)e.Data.GetData(typeof(Variable));
                AddField(field.Id, field.QualifiedFieldName);
            }
            Focus();
        }

        private void existingFunctionReconfigured(object sender, FunctionConfiguredEventArgs e)
        {
            if (!e.Canceled)
            {
                int uniqueifiedId = e.OriginalInstance.InstanceId;
                int newId = e.EditedInstance.InstanceId;
                string text = e.EditedInstance.ToDisplayString();

                Debug.Assert(Project.FunctionMapById.ContainsKey(uniqueifiedId), "map doesn't contain pre-edit ID");
                Debug.Assert(!Project.FunctionMapById.ContainsKey(newId), "newId is already in FunctionMapById");
                Debug.Assert(uniqueifiedId != newId,
                             "Clicking OK on edit resulteds in cloned function being returned so previous and new id must differ");

                Project.FunctionMapById.AddUnique(e.EditedInstance);
                UpdateFunctionFieldByUniqueId(uniqueifiedId, newId, text);
                Debug.Assert(!IsFieldIdInUse(uniqueifiedId));
                Project.FunctionMapById.Remove(uniqueifiedId);
                modified = true;
            }
        }

        private void newFunctionConfigured(object sender, FunctionConfiguredEventArgs e)
        {
            if (!e.Canceled)
            {
                int uniqueifiedId = e.OriginalInstance.InstanceId;
                int newId = e.EditedInstance.InstanceId;
                string text = e.EditedInstance.ToDisplayString();

                Debug.Assert(uniqueifiedId == newId,
                             "Clicking OK on insert results in both previous and new functions to be same reference.");

                Project.FunctionMapById.AddUnique(e.EditedInstance);
                InsertFunctionField(newId, text);
                modified = true;
            }
        }

        public void InsertFunction()
        {
            int instanceId = SelectedFunctionFieldInstanceId;
            if (Project.FunctionMapById.ContainsKey(instanceId))
            {
                EditSelectedFunctionConfiguration(instanceId);
            }
            else
            {
                var dialog = new InsertFunctionDialog(newFunctionConfigured);
                dialog.ShowDialog(Parent);
            }
        }

        public void InsertUploadedImage()
        {
            int instanceId = SelectedFunctionFieldInstanceId;
            if (Project.FunctionMapById.ContainsKey(instanceId))
            {
                EditSelectedFunctionConfiguration(instanceId);
            }
            else
            {
                IFunctionInfo functionInfo = FunctionLoader.Repository.Functions["display-image"];
                ConfigureFunctionDialog.Presenter.CreateFunction(functionInfo, newFunctionConfigured);
            }
        }

        public void EditSelectedFunctionConfiguration(int instanceId)
        {
            IFunction function = cloneFunctionToEnsureUniqueId(instanceId);

            if (function != null)
            {
                ConfigureFunctionDialog.Presenter.EditFunction(function, existingFunctionReconfigured);
            }
        }

        private IFunction cloneFunctionToEnsureUniqueId(int instanceId)
        {
            IFunction function = Project.FunctionMapById[instanceId];

            var converter = new XmlToFunctionConverter();
            function = converter.CloneFunction(function);
            Project.FunctionMapById.AddUnique(function);
            ChangeSelectedFunctionFieldIdToUniqueId(instanceId, function.InstanceId);

            // Commented out to fix Bug 517 -- another document may still hold a reference to this id in a textfield.
            // 
            //if (!IsFieldIdInUse(instanceId))
            //{
            //    Project.FunctionMapById.Remove(instanceId);
            //}

            modified = true; // text field's id changed because of cloning
            return function;
        }

        #endregion

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DocumentEditor
            // 
            this.Name = "DocumentEditor";
            this.Size = new System.Drawing.Size(424, 384);
            this.ResumeLayout(false);
        }

        #endregion
    }
}