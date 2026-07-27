// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Functions.Controls;
using Tawala.Functions.Runtime;
using Tawala.Functions.ViewPresenter;
using Tawala.Projects;
using Tawala.Projects.Fields;
using Tawala.ProjectUI;
using Tawala.TextEditor;
using Form=Tawala.Projects.Form;

namespace Tawala.Forms
{
    public partial class ItemTextEditor : TextEdit, IEditMenu
    {
        private readonly Timer entryTimer = new Timer();
        private int preferredHeight;
        private bool retainSelectionOnEntry;

        public ItemTextEditor()
        {
            InitializeComponent();
            FixUnexpectedScrollBehavior = true;
        }

        public new ItemViewBase Parent { get { return base.Parent as ItemViewBase; } set { base.Parent = value; } }

        public bool ValidAncestors { get { return base.Parent is ItemViewBase && Parent.Parent is FormItemContainer; } }

        public bool RetainSelectionOnEntry { set { retainSelectionOnEntry = value; } }

        #region IEditMenu Members

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
            if (CanPaste)
            {
                Paste();
            }
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SelectionHighlightRequiresFocus = true;
            //			ViewMode = Tawala.TextEditor.ViewMode.Simple; // fixes drag scrolling wierdness, but seems to have limited height and keeps some images from displaying
            ViewMode = ViewMode.Unlimited;
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            int fixedWidth = proposedSize.Width - ItemViewBase.LabelWidth;
            if (fixedWidth < 100)
            {
                fixedWidth = 100;
            }

            return new Size(fixedWidth, preferredHeight);
        }

        public override void OnChanged(EventArgs e)
        {
            int h = GetPreferredHeight();
            if (ValidAncestors && preferredHeight != h)
            {
                preferredHeight = h;

                setScrollOffset();

                Parent.Parent.PerformLayout();
            }

            base.OnChanged(e);
        }

        private void setScrollOffset()
        {
            if (Parent is TextItemView)
            {
                Parent.AutoScrollOffset = GetScrollOffset();
            }
        }

        public override void HandleDragEnter(DragEventArgs e)
        {
            if (supportedDragDataType(e))
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
            if (supportedDragDataType(e))
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
            // work around for WM_SETFOCUS blocking issues in ItemViewBase
            Parent.SetFocusToChild(this);

            var ptScreen = new Point(e.X, e.Y);

            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                Select(ptScreen, 0);
                Selection.Text = (string)e.Data.GetData(DataFormats.Text);
            }
            else if (e.Data.GetDataPresent(typeof(IPaletteField)))
            {
                Select(ptScreen, 0);
                InsertField((IPaletteField)e.Data.GetData(typeof(IPaletteField)));
            }
            else if (e.Data.GetDataPresent(typeof(Variable)))
            {
                Select(ptScreen, 0);
                InsertField((IPaletteField)e.Data.GetData(typeof(Variable)));
            }
        }

        private bool supportedDragDataType(DragEventArgs e)
        {
            return (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(typeof(IPaletteField)) ||
                    e.Data.GetDataPresent(typeof(Variable)));
        }

        public void InsertField(IPaletteField field)
        {
            AddField(field.Id, field.QualifiedFieldName);
            ForceOnChanged();
        }

        private void existingFunctionReconfigured(object sender, FunctionConfiguredEventArgs e)
        {
            if (!e.Canceled)
            {
                int uniqueifiedId = e.OriginalInstance.InstanceId;
                int newId = e.EditedInstance.InstanceId;
                string text = e.EditedInstance.ToDisplayString();

                Project.FunctionMapById.AddUnique(e.EditedInstance);
                UpdateFunctionFieldByUniqueId(uniqueifiedId, newId, text);
                Project.FunctionMapById.Remove(uniqueifiedId);

                if (Parent is TextItemView)
                {
                    var textItemView = Parent as TextItemView;
                    textItemView.ProjectTextItem.Rtf = GetRTF();
                }
            }
        }

        private void newFunctionConfigured(object sender, FunctionConfiguredEventArgs e)
        {
            if (!e.Canceled)
            {
                int uniqueifiedId = e.OriginalInstance.InstanceId;
                int newId = e.EditedInstance.InstanceId;
                string text = e.EditedInstance.ToDisplayString();

                Project.FunctionMapById.AddUnique(e.EditedInstance);
                InsertFunctionField(newId, text);

                if (Parent is TextItemView)
                {
                    var textItemView = Parent as TextItemView;
                    textItemView.ProjectTextItem.Rtf = GetRTF();
                }
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

            // Removed to fix Bug 517 -- another document may still hold a reference to this id in a textfield.
            // 
            //if (!IsFieldIdInUse(instanceId))
            //{
            //    Project.FunctionMapById.Remove(instanceId);
            //}

            return function;
        }

        private void timerEventHandler(Object timerObject, EventArgs timerEventArgs)
        {
            if (!retainSelectionOnEntry)
            {
                // fixes Bug 267: undesired selection of text when first clicking in control
                Selection.Length = 0;
                Focus();
            }

            retainSelectionOnEntry = false;
            entryTimer.Stop();
        }

        protected override void OnEnter(EventArgs e)
        {
            if (FindForm() is MDIFormView)
            {
                ((MDIFormView)FindForm()).TargetTextEditor = this;

                if (MouseButtons == MouseButtons.Left)
                {
                    entryTimer.Tick += timerEventHandler;
                    entryTimer.Interval = 100;
                    entryTimer.Start();
                }
            }

            base.OnEnter(e);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            attachEvents();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            detachEvents();

            base.OnHandleDestroyed(e);
        }

        private void attachEvents()
        {
            Project.Events.FormItemAdded += events_FormItemChanged;
            Project.Events.FormItemRemoved += events_FormItemChanged;
            Project.Events.FormItemChanged += events_FormItemChanged;
            Project.Events.ComponentRenamed += events_ComponentRenamed;
        }

        private void detachEvents()
        {
            Project.Events.FormItemAdded -= events_FormItemChanged;
            Project.Events.FormItemRemoved -= events_FormItemChanged;
            Project.Events.FormItemChanged -= events_FormItemChanged;
            Project.Events.ComponentRenamed -= events_ComponentRenamed;
        }

        private void events_FormItemChanged(object sender, FormItemEventArgs e)
        {
            updateFieldsAndFunctions();
        }

        private void events_ComponentRenamed(object sender, ComponentRenamedEventArgs e)
        {
            if (e.Component is Form)
            {
                updateFieldsAndFunctions();
            }
        }

        private void updateFieldsAndFunctions()
        {
            if (ContainsStandardFields())
            {
                //UpdateFieldNames(Project.FieldMapById.GetQualifiedFieldDictionary());
                UpdateFieldNames(Project.FieldMapById.GetQualifiedFieldDictionary(getFieldIds()));
            }

            if (ContainsFunctionFields())
            {
                UpdateFunctionFieldText(Project.FunctionMapById.GetDisplayStringDictionary());
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((IEditMenu)this).Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((IEditMenu)this).Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((IEditMenu)this).Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((IEditMenu)this).Delete();
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            cutToolStripMenuItem.Enabled = ((IEditMenu)this).CanCut();
            copyToolStripMenuItem.Enabled = ((IEditMenu)this).CanCopy();
            pasteToolStripMenuItem.Enabled = ((IEditMenu)this).CanPaste();
            deleteToolStripMenuItem.Enabled = ((IEditMenu)this).CanCut();
            selectAllToolStripMenuItem.Enabled = GetText().Length > 0;
            e.Cancel = false;
        }

        // unlike the other short cuts on context menu, Ctrl-A fails to work
        // even though the context menu defines it as a shortcut.  Giving up on the mystery
        // and doing what works.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool keystrokeHandled = false;

            switch (keyData)
            {
                case Keys.Control | Keys.A:
                    SelectAll();
                    keystrokeHandled = true;
                    break;

                case Keys.Control | Keys.B:
                    ToggleBold();
                    keystrokeHandled = true;
                    break;

                case Keys.Control | Keys.I:
                    ToggleItalic();
                    keystrokeHandled = true;
                    break;

                case Keys.Control | Keys.U:
                    ToggleUnderline();
                    keystrokeHandled = true;
                    break;

                default:
                    break;
            }

            return keystrokeHandled ? true : base.ProcessCmdKey(ref msg, keyData);
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectAll();
        }
    }
}