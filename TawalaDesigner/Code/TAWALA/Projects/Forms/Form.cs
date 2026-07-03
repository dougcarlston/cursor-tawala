// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.Text;
using Tawala.Common;
using Tawala.Projects.Components;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.Projects.Properties;
using Tawala.XmlSupport;
using Process=Tawala.Projects.Processes.Process;

namespace Tawala.Projects
{
    /// <summary>
    /// Component that encapsulates a Form component.
    /// </summary>
    [Serializable]
    public class Form : Component, IForm
    {
        public static readonly Form NULL = new NullForm("Null Form");

        private readonly int id = Project.NextUniqueID;

        /// <summary>
        /// Property to track if there is an active Skip Instruction item
        /// </summary>
        /// <remarks>
        /// The Skip Instructions view in the UI needs to know what the "hot" item is
        /// </remarks>
        [NonSerialized]
        private SkipInstructionsItem activeSkipInstructionsItem;

        [NonSerialized]
        private IProcess _connectedPreProcess;

        [NonSerialized]
        private IProcess _connectedPostProcess;

        /// <summary>
        /// Indicates whether the Form is to be used for data entry only (rather than for data collection).
        /// </summary>
        private bool _dataEntryOnly;

        private string _dataSourceName = string.Empty;
        private FormItemList _formItemList;

        /// <summary>
        /// a list of SkipToDestinationItems for this Form
        /// </summary>
        [NonSerialized]
        private SkipToDestinationList skipToDestinations = new SkipToDestinationList();

        private bool startPoint;

        public Form(string initialName) : base(initialName)
        {
            commonConstruct();
        }

        public Form(IXmlElement element)
        {
            Name = element.GetAttribute("name");
            Debug.Assert(Name != null);

            startPoint = (element.HasAttribute("startPoint") ? (element.GetAttribute("startPoint") == "true") : true);
            _dataEntryOnly = (element.HasAttribute("dataEntryOnly") ? (element.GetAttribute("dataEntryOnly") == "true") : false);
            _dataSourceName = (element.HasAttribute("dataSourceName") ? element.GetAttribute("dataSourceName") : string.Empty);
            BlockBackButton = (element.HasAttribute("blockBackButton") ? (element.GetAttribute("blockBackButton") == "true") : false);

            if (element.HasAttribute("preProcess"))
            {
                _connectedPreProcess = Project.Current.GetProcess(element.GetAttribute("preProcess"));
            }

            commonConstruct();
        }

        public int Id { get { return id; } }

        public bool HasConnectedProcess { get { return (_connectedPostProcess != null); } }

        public bool HasConnectedPreProcess { get { return (_connectedPreProcess != null); } }

        #region IForm Members

        public bool BlockBackButton { get; set; }

        public override string UserVisibleComponentTypeName { get { return Resources.FormComponentVisibleTypeName; } }

        public FormItemList ItemList { get { return _formItemList; } set { _formItemList = value; } }

        public bool StartingPoint
        {
            get { return startPoint; }
            set
            {
                if (startPoint != value)
                {
                    startPoint = value;
                    Project.Events.RaiseFormChangedEvent(new ComponentEventArgs(this));
                }
            }
        }

        public bool DataEntryOnly
        {
            get { return _dataEntryOnly; }
            set
            {
                if (_dataEntryOnly != value)
                {
                    _dataEntryOnly = value;
                    Project.Events.RaiseFormChangedEvent(new ComponentEventArgs(this));
                }
            }
        }

        public string DataSourceName { get { return _dataSourceName; } set { _dataSourceName = value; } }

        public bool IsDataSource { get { return _dataSourceName != string.Empty; } }

        public FieldList GetFields()
        {
            var fieldList = new FieldList();

            fieldList.Add(_formItemList);

            return fieldList;
        }

    	private FieldList formItemFieldList = null;
        public FieldList GetFormItemFields()
        {
			//var fieldList = new FieldList();

			//foreach (IField field in _formItemList.RecursiveEnumerator)
			//{
			//    if (isFormItemField(field))
			//    {
			//        fieldList.Add(field);
			//    }
			//}

			//return fieldList;

			if (formItemFieldList == null)
			{
				formItemFieldList = new FieldList();
			}

			// kludge to reduce load time of large projects
			// jdf - 5/13/10
			if (Project.LoadingProjectXml && formItemFieldList.Count > 0)
			{
				return formItemFieldList;
			}

			formItemFieldList.Clear();
			foreach (IField field in _formItemList.RecursiveEnumerator)
			{
				if (isFormItemField(field))
				{
					formItemFieldList.Add(field);
				}
			}
			return formItemFieldList;
        }

        public FieldList GetFormItemFieldsAndRecordVariables()
        {
            var fieldList = new FieldList();

            foreach (IField field in _formItemList.RecursiveEnumerator)
            {
                if (isFormItemField(field))
                {
                    fieldList.Add(field);
                }
            }

            foreach (Variable field in getVariablesFromConnectedProcess())
            {
                fieldList.Add(field);
            }

            return fieldList;
        }

        public FieldList GetAllFields()
        {
            var fieldList = new FieldList();

            foreach (IField field in _formItemList.RecursiveEnumerator)
            {
                if (isFormItemField(field))
                {
                    fieldList.Add(field);
                }
            }

            return fieldList;
        }

        /// <summary>
        /// Get list of fields in form
        /// </summary>
        public FieldList GetMCFields()
        {
            var fieldList = new FieldList();

            foreach (IFormItem item in _formItemList)
            {
                if (item is IMcqItem)
                {
                    fieldList.AddUnique(item as IMcqItem);
                }
            }

            return fieldList;
        }

        /// <summary>
        /// Gets the default label corresponding to the specified form item
        /// </summary>
        public string GetDefaultLabel(IFormItem formItem)
        {
            return _formItemList.GetDefaultLabel(formItem);
        }

        /// <summary>
        /// Gets the form item corresponding to the specified id
        /// </summary>
        public IFormItem GetFormItem(int formItemId)
        {
            return _formItemList.GetItemById(formItemId);
        }

        /// <summary>
        /// Gets the form item corresponding to the specified label
        /// </summary>
        public IFormItem GetFormItem(string formItemLabel)
        {
            foreach (IFormItem item in _formItemList)
            {
                if (GetDefaultLabel(item) == formItemLabel || item.AlternateLabel == formItemLabel)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// looks for the existence of a statement in any of this Form's Skip Instructions
        /// </summary>
        public Process GetSkipInstructions(ProcessStatement statement)
        {
            // for each FormItem
            foreach (FormItem item in ItemList)
            {
                // if it's a SkipInstructionsItem
                var skipItem = item as SkipInstructionsItem;
                if (skipItem != null)
                {
                    // check its process lines for the statement
                    foreach (ProcessLine line in skipItem.Instructions.Lines)
                    {
                        if (line.Statement == statement)
                        {
                            return skipItem.Instructions;
                        }
                    }
                }
            }

            return null;
        }

        public SkipInstructionsItem ActiveSkipToItem
        {
            get
            {
                // make sure the active item's in the Form is an error
                if (!_formItemList.Contains(activeSkipInstructionsItem))
                {
                    activeSkipInstructionsItem = null;
                }

                return activeSkipInstructionsItem;
            }
            set
            {
                // trying to set the active item to one that's not in the Form is an error
                activeSkipInstructionsItem = !_formItemList.Contains(value) ? null : value;
            }
        }

        public SkipToDestinationList SkipToDestinations
        {
            get
            {
                // this is not serialized -- construction doesn't occur on deserialization so the above "new"
                // doesn't occur in that case.
                if (skipToDestinations == null)
                {
                    skipToDestinations = new SkipToDestinationList();
                }

                // we regenerate the list and its labels whenever requested
                skipToDestinations.Clear();

                // walk the FormItem list
                foreach (FormItem formItem in ItemList)
                {
                    // add all text and question items to the skipDestinations list
                    if (formItem.IsTextItem || formItem.IsQuestionItem || formItem is IHeadingItem)
                    {
                        skipToDestinations.Add(new SkipToDestinationItem(formItem));
                    }
                }

                // always add a "end of Form" item
                // (the default constructor creates an end-of-form destination item)
                skipToDestinations.Add(new SkipToDestinationItem());

                return skipToDestinations;
            }
        }

        public override string ToString()
        {
            if (NULL != this && (Project.Current == null || Project.Current.AllForms.Contains(this)))
            {
                return base.ToString();
            }
            return NULL.Name;
        }

        public override string ToXml()
        {
            const string xmlFormStartTag = "<form name=\"{0}\" startPoint=\"{1}\"";
            const string xmlThemePath = " themePath=\"{0}\"";
            const string xmlDataEntryOnly = " dataEntryOnly=\"{0}\"";
            const string xmlDataSourceName = " dataSourceName=\"{0}\"";
            const string xmlFormProcess = " process=\"{0}\"";
            const string xmlFormPreProcess = " preProcess=\"{0}\"";
            const string xmlFormBlockBackButton = " blockBackButton=\"{0}\"";
            const string xmlCloseBlock = ">\r\n";
            const string xmlFormCloseTag = "</form>\r\n";

            var xmlString = new StringBuilder();

            xmlString.AppendFormat(xmlFormStartTag, XMLStringFormatter.EscapeAttributeText(Name), StartingPoint.ToString().ToLower());

            xmlString.AppendFormat(xmlThemePath, XMLStringFormatter.EscapeAttributeText(Project.Current.ThemePath));

            if (_dataEntryOnly)
            {
                xmlString.AppendFormat(xmlDataEntryOnly, _dataEntryOnly.ToString().ToLower());
            }

            if (IsDataSource)
            {
                xmlString.AppendFormat(xmlDataSourceName, _dataSourceName);
            }

            if (_connectedPostProcess != null)
            {
                xmlString.AppendFormat(xmlFormProcess, XMLStringFormatter.EscapeAttributeText(_connectedPostProcess.Name));
            }

            if (_connectedPreProcess != null)
            {
                xmlString.AppendFormat(xmlFormPreProcess, XMLStringFormatter.EscapeAttributeText(_connectedPreProcess.Name));
            }

            xmlString.AppendFormat(xmlFormBlockBackButton, BlockBackButton.ToString().ToLower());

            xmlString.Append(xmlCloseBlock);
            xmlString.Append(ItemList.ToXml());

            return xmlString.Append(xmlFormCloseTag).ToString();
        }

        public string ToValueXml()
        {
            return ToString();
        }

        public string FormattedString { get { return Name; } }

        public IProcess ConnectedPostProcess { get { return _connectedPostProcess; } set { _connectedPostProcess = value; } }

        public IProcess ConnectedPreProcess { get { return _connectedPreProcess; } set { _connectedPreProcess = value; } }

        public void ResolveProcessReferences()
        {
            throw new NotImplementedException();
        }

        #endregion

        private void commonConstruct()
        {
            _formItemList = new FormItemList(this);
        }

        private VariableList getVariablesFromConnectedProcess()
        {
            var variables = new VariableList();

            if (_connectedPostProcess != null)
            {
                variables = _connectedPostProcess.Variables;
            }

            return variables;
        }

        private static bool isFormItemField(IField field)
        {
            return field is McqItem || field is Blank || field is HiddenField || field is IFileUploadItem;
        }

        #region Nested type: NullForm

        [Serializable]
        private class NullForm : Form
        {
            public NullForm(string name) : base(name)
            {
            }
        }

        #endregion
    }
}