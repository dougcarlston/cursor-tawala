// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Projects;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Process=Tawala.Projects.Processes.Process;
using WindowsForm = System.Windows.Forms.Form;

namespace Tawala.ProjectUI
{
    public partial class FieldsPalette : UserControl
    {
        private static readonly FieldPaletteEventRouter eventRouter = new FieldPaletteEventRouter();

        public EventHandler<FieldsPaletteStatusEventArgs> StatusChanged;

        private static FieldsPalette palette;
        private FunctionFormCollection conditionsForms = new FunctionFormCollection();
        private bool configureFunctionActive;
        private RecordSetNode recordSetNode;
        private bool refreshList;

        public FieldsPalette()
        {
            palette = this;
            InitializeComponent();
            Dock = DockStyle.Fill;

            Project.Events.FormItemAdded += events_FormItemAdded;
            Project.Events.FormItemChanged += events_FormItemChanged;
            Project.Events.FormItemRemoved += events_FormItemChanged;
            Project.Events.ComponentAdded += events_ComponentAdded;
            Project.Events.ComponentRemoved += events_ComponentRemoved;
            Project.Events.ComponentRenamed += events_ComponentRenamed;
            Project.Events.ProcessVariableListChanged += events_ProcessVariableListChanged;
            Project.Events.NewProject += events_ProjectChanged;
            Project.Events.ProjectOpened += events_ProjectChanged;
            Project.Events.ProcessChanged += events_ProcessChanged;

            Project.Events.MCItemSelected += events_MCItemSelected;

            Project.Events.ProcessConnectedToForm += events_ProcessConnectedToForm;
            Project.Events.ProcessDisconnectedFromForm += events_ProcessDisconnectedFromForm;

            Application.Idle += application_Idle;
        }

        public static FieldsPalette Palette { get { return palette; } }

        public TreeView FieldsTreeView { get { return fieldsTreeView; } }

        public TreeNode SelectedNode { get { return fieldsTreeView != null ? fieldsTreeView.SelectedNode : null; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static bool ConfigureFunctionActive
        {
            get { return palette != null ? palette.configureFunctionActive : false; }
            set
            {
                if (palette != null)
                {
                    palette.configureFunctionActive = value;
                    palette.RefreshFieldList();
                }
            }
        }

        public FunctionFormCollection ConditionsForms
        {
            get { return conditionsForms; }

            set
            {
                if (!conditionsForms.Equals(value))
                {
                    conditionsForms = value;
                    RefreshFieldList();
                }
            }
        }

        public event TreeNodeMouseClickEventHandler FieldNodeDoubleClick { add { eventRouter.SetDoubleClick(value); } remove { eventRouter.RemoveDoubleClick(value); } }

        public void RefreshFieldList()
        {
            addFormItems();
            addVariables();
            addRecordItems();
            addRecordSetItems();
            addChoices();
        }

        public void RefreshPalette()
        {
            FieldsTreeView.Nodes.Clear();
            refreshList = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            refreshList = true;
        }

        private void application_Idle(object sender, EventArgs e)
        {
            if (refreshList && Project.Current != null)
            {
                RefreshFieldList();

                refreshList = false;
            }
        }

        private void addFormItems()
        {
            foreach (IForm form in Project.Current.AllForms)
            {
                TreeNode formNode = fieldsTreeView.Nodes[form.Name];

                if (form is ExternalForm && !ConfigureFunctionActive)
                {
                    if (formNode != null)
                    {
                        fieldsTreeView.Nodes.Remove(formNode);
                    }
                    continue;
                }

                if (formNode == null)
                {
                    formNode = new TreeNode(form.Name);
                    formNode.Name = form.Name;

                    if (form is ExternalForm)
                    {
                        formNode.ForeColor = Color.DarkBlue;
                    }

                    fieldsTreeView.Nodes.Add(formNode);
                }

                FieldList responseFields = form.GetFormItemFields();

                for (int i = 0; i < responseFields.Count; i++)
                {
                    TreeNode fieldNode = makeFormFieldNode(responseFields[i]);

                    if (i < formNode.Nodes.Count)
                    {
                        replaceSubNode(formNode, i, fieldNode);
                    }
                    else
                    {
                        addSubNode(formNode, fieldNode);
                    }
                }

                int extraNodeCount = formNode.Nodes.Count - responseFields.Count;

                removeExtraSubNodes(formNode, extraNodeCount);

                if (formNodeToExpand == formNode.Name)
                {
                    expandFormNodeOnlyOnce(formNode);
                }
            }
        }

        private void expandFormNodeOnlyOnce(TreeNode formNode)
        {
            formNode.Expand();
            formNodeToExpand = "";
        }

        private static TreeNode makeFormFieldNode(IField field)
        {
            var fieldNode = new TreeNode(field.FieldName);
            fieldNode.Tag = field;

            return fieldNode;
        }

        private void addVariables()
        {
            const string variablesNodeName = "Variables";
            TreeNode variablesNode = fieldsTreeView.Nodes[variablesNodeName];

            if (projectHasVariables())
            {
                if (variablesNode == null)
                {
                    variablesNode = new TreeNode(variablesNodeName);
                    variablesNode.Name = variablesNodeName;

                    fieldsTreeView.Nodes.Add(variablesNode);
                }

                SortedList<string, Variable> sortedVariables = getSortedVariables();

                for (int i = 0; i < sortedVariables.Count; i++)
                {
                    TreeNode variableNode = makeVariableNode(sortedVariables.Values[i]);

                    if (i < variablesNode.Nodes.Count)
                    {
                        replaceSubNode(variablesNode, i, variableNode);
                    }
                    else
                    {
                        addSubNode(variablesNode, variableNode);
                    }
                }

                int extraNodeCount = variablesNode.Nodes.Count - sortedVariables.Count;

                removeExtraSubNodes(variablesNode, extraNodeCount);
            }
            else
            {
                removeNode(variablesNode);
            }
        }

        private static bool projectHasVariables()
        {
            return (Project.Current != null && Project.Current.AllVariables.Count > 0);
        }

        private static void removeExtraSubNodes(TreeNode parentNode, int extraNodeCount)
        {
            for (int i = 0; i < extraNodeCount; i++)
            {
                parentNode.Nodes.RemoveAt(parentNode.Nodes.Count - 1);
            }
        }

        private static void addSubNode(TreeNode parentNode, TreeNode newSubNode)
        {
            parentNode.Nodes.Add(newSubNode);
        }

        private static void replaceSubNode(TreeNode parentNode, int subNodeIndex, TreeNode newSubNode)
        {
            parentNode.Nodes.RemoveAt(subNodeIndex);
            parentNode.Nodes.Insert(subNodeIndex, newSubNode);
        }

        private static void removeNode(TreeNode parentNode)
        {
            if (parentNode != null)
            {
                parentNode.Nodes.Clear();
                parentNode.Remove();
            }
        }

        private static void renameNode(TreeNode node, string newName)
        {
            if (node != null)
            {
                node.Name = newName;
                node.Text = newName;
            }
        }

        private static SortedList<string, Variable> getSortedVariables()
        {
            var sortedVariables = new SortedList<string, Variable>();

            foreach (Variable variable in Project.Current.AllVariables)
            {
                sortedVariables.Add(variable.FieldName, variable);
            }

            return sortedVariables;
        }

        private static TreeNode makeVariableNode(Variable variable)
        {
            var variableNode = new TreeNode(variable.FieldName);
            variableNode.ToolTipText = variable.GetToolTipText();
            variableNode.Tag = variable;

            return variableNode;
        }

        private void addRecordItems()
        {
            if (conditionsFormsExistWithFields())
            {
                addConditionsRecordItems();
            }
            else if (process != null)
            {
                addProcessRecordItems();
            }
            else
            {
                removeAllRecordNodes();
            }
        }

        private void addProcessRecordItems()
        {
            if (Project.Current.ProcessList.Contains(process))
            {
                foreach (Record record in getForEachRecordsInProcessOrder(process, processLineIndex))
                {
                    FieldList allFields = getAllFields(process.GetRecordSet(record).Forms);

                    buildRecordSubTree(record, allFields);
                }
            }

            removeUnusedRecordNodes();
        }

        private bool conditionsFormsExistWithFields()
        {
            return conditionsForms != null && conditionsForms.GetAllFields().Count > 0;
        }

        private void addConditionsRecordItems()
        {
            if (conditionsForms != null)
            {
                var fields = new FieldList();
                getFormFields(conditionsForms, fields);

                if (fields.Count > 0)
                {
                    buildRecordSubTree(new Record(FieldUtil.DefaultRecordQualifierName), fields);
                }
            }
        }

        private void buildRecordSubTree(Record record, FieldList fields)
        {
            TreeNode recordNode = makeRecordNode(record);

            for (int i = 0; i < fields.Count; i++)
            {
                TreeNode fieldNode = makeRecordFieldNode(record, fields[i] as IPaletteField);

                if (i < recordNode.Nodes.Count)
                {
                    replaceSubNode(recordNode, i, fieldNode);
                }
                else
                {
                    addSubNode(recordNode, fieldNode);
                }
            }

            removeExtraSubNodes(recordNode, recordNode.Nodes.Count - fields.Count);
        }

        private void removeAllRecordNodes()
        {
            foreach (TreeNode node in fieldsTreeView.Nodes)
            {
                if (isRecordNode(node))
                {
                    node.Remove();
                }
            }
        }

        private void removeUnusedRecordNodes()
        {
            if (process != null)
            {
                VariableList forEachRecords = getForEachRecordsInProcessOrder(process, processLineIndex);
                var usedRecordNodes = new Collection<TreeNode>();

                foreach (TreeNode node in fieldsTreeView.Nodes)
                {
                    foreach (Record record in forEachRecords)
                    {
                        if (node.Name == record.FieldName)
                        {
                            usedRecordNodes.Add(node);
                        }
                    }
                }

                foreach (TreeNode node in fieldsTreeView.Nodes)
                {
                    if (isRecordNode(node))
                    {
                        if (!usedRecordNodes.Contains(node))
                        {
                            node.Remove();
                        }
                    }
                }
            }
        }

        private void addRecordSetItems()
        {
            if (process == null)
            {
                removeRecordSetNode();
            }
            else
            {
                if (Project.Current.ProcessList.Contains(process))
                {
                    TreeNode recordSetNode = makeRecordSetNode();

                    if (recordSetNode != null)
                    {
                        recordSetNode.Expand();
                    }
                }
            }
        }

        private void removeRecordSetNode()
        {
            if (recordSetNode != null)
            {
                recordSetNode.Remove();
            }
        }

        private static bool isRecordNode(TreeNode node)
        {
            return (!isFormNode(node) && !isVariablesNode(node) && !isChoicesNode(node));
        }

        private static bool isFormNode(TreeNode node)
        {
            foreach (IForm form in Project.Current.AllForms)
            {
                if (node.Name == form.Name)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool isVariablesNode(TreeNode node)
        {
            if (node.Name == "Variables")
            {
                return true;
            }

            return false;
        }

        private static bool isChoicesNode(TreeNode node)
        {
            if (node.Name == "Choices")
            {
                return true;
            }

            return false;
        }

        private TreeNode makeRecordNode(Record record)
        {
            TreeNode recordNode = fieldsTreeView.Nodes[record.FieldName];

            if (recordNode == null)
            {
                recordNode = new TreeNode(record.FieldName);
                recordNode.Name = record.FieldName;
                recordNode.Tag = record;

                fieldsTreeView.Nodes.Add(recordNode);
            }

            return recordNode;
        }

        private static TreeNode makeRecordFieldNode(Record record, IPaletteField field)
        {
            var fieldNode = new TreeNode(field.QualifiedFieldName);
            fieldNode.Tag = new RecordField(record, field);

            return fieldNode;
        }

        private RecordSetNode makeRecordSetNode()
        {
            if (getStatement == null)
            {
                if (recordSetNode != null)
                {
                    recordSetNode.Remove();
                }
            }
            else
            {
                if (getStatement.Records != null)
                {
                    if (recordSetNode == null)
                    {
                        recordSetNode = new RecordSetNode(getStatement);
                    }
                }

                recordSetNode.ReferencedGetStatement = getStatement;
                recordSetNode.Text = getStatement.Records.FieldName;
                fieldsTreeView.Nodes.Add(recordSetNode);
                recordSetNode.PopulateSubNodes();
            }

            return recordSetNode;
        }

        /// <summary>
        /// Returns a list of fields that are in any Form in the specified list.
        /// </summary>
        private static FieldList getAllFields(FormList forms)
        {
            var allFields = new FieldList();

            foreach (IForm form in forms)
            {
                getFormFields(form, allFields);
            }

            return allFields;
        }

        private static void getFormFields(IForm form, FieldList allFields)
        {
            if (form != null)
            {
                foreach (IField field in form.GetAllFields())
                {
                    var paletteField = field as IPaletteField;
                    if (paletteField != null)
                    {
                        if (allFields.IndexOf(paletteField.QualifiedFieldName) == -1)
                        {
                            allFields.Add(paletteField);
                        }
                    }
                }
            }
        }

        private static void getFormFields(FunctionFormCollection forms, FieldList allFields)
        {
            foreach (IForm form in forms)
            {
                getFormFields(form, allFields);
            }
        }

        /// <summary>
        /// Returns a list of records corresponding to the FOR EACH statements in the specified process enclosing the specified process line.
        /// The records in the list are in the same order as they appear in the process.
        /// </summary>
        private static VariableList getForEachRecordsInProcessOrder(Process process, int processLineIndex)
        {
            var records = new VariableList();

            if (process != null)
            {
                foreach (IField record in process.GetForEachRecords(processLineIndex))
                {
                    records.Insert(0, (Variable)record);
                }
            }

            return records;
        }

        private void addChoices()
        {
            removeChoicesNodes();

            string choicesNodeName = "Choices";

            if (mcItem != null)
            {
                if (mcItem.Choices.Count > 0)
                {
                    var choicesNode = new TreeNode(choicesNodeName);
                    choicesNode.Name = choicesNodeName;
                    fieldsTreeView.Nodes.Add(choicesNode);

                    for (int i = 0; i < mcItem.Choices.Count; i++)
                    {
                        choicesNode.Nodes.Add(makeChoiceNode(new ChoiceField(mcItem.Choices.GetLabel(i))));
                    }

                    choicesNode.Expand();
                }
            }
        }

        private void removeChoicesNodes()
        {
            foreach (TreeNode node in fieldsTreeView.Nodes)
            {
                if (isChoicesNode(node))
                {
                    node.Remove();
                }
            }
        }

        private static TreeNode makeChoiceNode(ChoiceField field)
        {
            var choiceNode = new TreeNode(field.FieldName);
            choiceNode.Tag = field;

            return choiceNode;
        }

        public void ClearConditionsForms()
        {
            if (conditionsForms != FunctionFormCollection.NULL)
            {
                conditionsForms = FunctionFormCollection.NULL;
            }
        }

        #region Project Events

        private string formNodeToExpand = "";
        private GetStatement getStatement;
        private IMcqItem mcItem;
        private Process process;
        private int processLineIndex;

        private void events_FormItemAdded(object sender, FormItemEventArgs e)
        {
            if (firstPaletteItemAdded(e.Form))
            {
                formNodeToExpand = e.Form.Name;
            }

            refreshList = true;
        }

        private static bool firstPaletteItemAdded(IForm form)
        {
            int paletteItemCount = 0;

            foreach (IFormItem item in form.ItemList)
            {
                if (item is IFibItem || item is IMcqItem || item is IHiddenField || item is IFileUploadItem)
                {
                    paletteItemCount++;
                }
            }

            return (paletteItemCount == 1);
        }

        private void events_FormItemChanged(object sender, FormItemEventArgs e)
        {
            refreshList = true;
        }

        private void events_ComponentAdded(object sender, ComponentEventArgs e)
        {
            if (e.Component is IForm || e.Component is Process)
            {
                refreshList = true;
            }
        }

        private void events_ComponentRemoved(object sender, ComponentEventArgs e)
        {
            if (e.Component is IForm || e.Component is Process)
            {
                removeNode(fieldsTreeView.Nodes[e.Component.Name]);
                refreshList = true;
            }
        }

        private void events_ComponentRenamed(object sender, ComponentRenamedEventArgs e)
        {
            if (e.Component is IForm)
            {
                renameNode(fieldsTreeView.Nodes[e.OldName], e.Component.Name);
                refreshList = true;
            }
            else if (e.Component is Process)
            {
                refreshList = true;
            }
        }

        private void events_ProcessVariableListChanged(object sender, ComponentEventArgs e)
        {
            if (e.Component is IForm || e.Component is Process)
            {
                refreshList = true;
            }
        }

        private void events_ProjectChanged(object sender, ProjectEventArgs e)
        {
            FieldsTreeView.Nodes.Clear();
            refreshList = true;
        }

        private void events_ProcessChanged(object sender, ProcessEventArgs e)
        {
            process = e.Component as Process;

            if (process != null)
            {
                if (processLineIndexSpecified(e))
                {
                    processLineIndex = e.ProcessLineIndex;
                }

                getStatement = process.ActiveGetStatement;
            }

            refreshList = true;
        }

        private static bool processLineIndexSpecified(ProcessEventArgs e)
        {
            return e.ProcessLineIndex != -1;
        }

        private void events_MCItemSelected(object sender, MCItemEventArgs e)
        {
            mcItem = getMCItemWithMostChoices(e.Items);

            refreshList = true;
        }

        private static IMcqItem getMCItemWithMostChoices(IMcqItem[] items)
        {
            IMcqItem mcItemMost = null;

            int count = 0;
            foreach (IMcqItem item in items)
            {
                if (item != null && item.Choices.Count > count)
                {
                    mcItemMost = item;
                    count = item.Choices.Count;
                }
            }

            return mcItemMost;
        }

        private void events_ProcessDisconnectedFromForm(object sender, ProcessConnectionArgs e)
        {
            refreshList = true;
        }

        private void events_ProcessConnectedToForm(object sender, ProcessConnectionArgs e)
        {
            refreshList = true;
        }

        #endregion

        protected void OnStatusChanged(FieldsPaletteStatusEventArgs args)
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, args);
            }
        }

        #region Fields TreeView events

        private void fieldsTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var node = e.Item as TreeNode;
                if (node != null && node.Tag is IField)
                {
                    Debug.Assert(node.Tag is IPaletteField);

                    fieldsTreeView.SelectedNode = node;
                    DataObject dataObject = createDataObject(node);

                    OnStatusChanged(new FieldsPaletteStatusEventArgs(FieldsPaletteStatusChange.BeginNodeDrag));
                    DoDragDrop(dataObject, DragDropEffects.Copy);
                    OnStatusChanged(new FieldsPaletteStatusEventArgs(FieldsPaletteStatusChange.EndNodeDrag));
                }
            }
        }

        private static DataObject createDataObject(TreeNode node)
        {
            var dataObject = new DataObject();

            // GetType() returns the exact runtime type of node.Tag (Variable, RecordField, etc)

            dataObject.SetData(node.Tag.GetType(), node.Tag);

            var assignable = node.Tag as IAssignableField;

            if (assignable != null)
            {
                dataObject.SetData(typeof(IAssignableField), node.Tag);
            }

            dataObject.SetData(typeof(IPaletteField), node.Tag as IPaletteField);

            var anyField = node.Tag as IAnyField;
            dataObject.SetText("#Field# id=" + anyField.Id, TextDataFormat.Text);

            return dataObject;
        }

        private void fieldsTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            raiseFieldNodeDoubleClickEvent(sender, e);
        }

        private void raiseFieldNodeDoubleClickEvent(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.Node.Parent != null && e.Button == MouseButtons.Left)
            {
                eventRouter.InvokeDoubleClick(sender, e);
            }
        }

        private void fieldsTreeView_KeyPress(object sender, KeyPressEventArgs e)
        {
            TreeNode node = fieldsTreeView.SelectedNode;

            if (node != null && node.Parent != null && e.KeyChar == 13)
            {
                raiseFieldNodeDoubleClickEvent(sender, new TreeNodeMouseClickEventArgs(node, MouseButtons.Left, 2, 0, 0));
            }
        }

        private void fieldsTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
        }

        #endregion
    }

    /// <summary>
    /// The top-level node representing the RecordSet contained in a GET statement.
    /// </summary>
    [Serializable]
    public class RecordSetNode : TreeNode
    {
        private GetStatement referencedGetStatement;

        public RecordSetNode(GetStatement getStatement) : base(getStatement.Records.FieldName)
        {
            ReferencedGetStatement = getStatement;

            PopulateSubNodes();
        }

        public GetStatement ReferencedGetStatement { set { Tag = referencedGetStatement = value; } }

        public void PopulateSubNodes()
        {
            FieldList allFields = getAllFields(referencedGetStatement.Records.Forms);

            for (int i = 0; i < allFields.Count; i++)
            {
                TreeNode fieldNode = makeRecordSetFieldNode(allFields[i] as IPaletteField);

                if (i < Nodes.Count)
                {
                    replaceSubNode(i, fieldNode);
                }
                else
                {
                    addSubNode(fieldNode);
                }
            }

            removeExtraSubNodes(Nodes.Count - allFields.Count);
        }

        /// <summary>
        /// Returns a list of fields that are in any Form in the specified list.
        /// </summary>
        private static FieldList getAllFields(FormList forms)
        {
            var allFields = new FieldList();

            foreach (IForm form in forms)
            {
                foreach (IField field in form.GetAllFields())
                {
                    if (field is IPaletteField)
                    {
                        if (allFields.IndexOf(field.QualifiedFieldName) == -1)
                        {
                            allFields.Add(field);
                        }
                    }
                }
            }

            return allFields;
        }

        private TreeNode makeRecordSetFieldNode(IPaletteField field)
        {
            var fieldNode = new TreeNode(field.QualifiedFieldName);
            fieldNode.Tag = new RecordSetField(referencedGetStatement.Records, field, false);

            return fieldNode;
        }

        private void addSubNode(TreeNode newSubNode)
        {
            Nodes.Add(newSubNode);
        }

        private void replaceSubNode(int subNodeIndex, TreeNode newSubNode)
        {
            Nodes.RemoveAt(subNodeIndex);
            Nodes.Insert(subNodeIndex, newSubNode);
        }

        private void removeExtraSubNodes(int extraNodeCount)
        {
            for (int i = 0; i < extraNodeCount; i++)
            {
                Nodes.RemoveAt(Nodes.Count - 1);
            }
        }
    }
}