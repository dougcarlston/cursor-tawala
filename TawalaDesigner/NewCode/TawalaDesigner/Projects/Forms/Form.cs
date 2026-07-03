// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms
{
	[Serializable]
    public class Form : IForm
	{
		private string name;
		private bool startPoint;
		private FormItemList items;

		[NonSerialized]
		private IProcess connectedProcess;
        [NonSerialized]
        private IProcess connectedPreProcess;

		[NonSerialized]
		private SkipToDestinationList skipToDestinations = new SkipToDestinationList();

		public Form(string name)
		{
            Name = name;
            items = new FormItemList(this);
            DataSourceName = string.Empty;
		}

		public Form(IXmlElement element)
		{
            DataSourceName = string.Empty;

            Name = element.GetAttribute("name");
			startPoint = (element.GetAttribute("startPoint") == "true");

            if (element.HasAttribute("dataEntryOnly"))
            {
                dataEntryOnly = element.GetAttribute("dataEntryOnly") == "true";
            }

            if (element.HasAttribute("dataSourceName"))
            {
                DataSourceName = element.GetAttribute("dataSourceName");
            }

            if (element.HasAttribute("process"))
			{
				connectedProcess = new Process(element.GetAttribute("process"));
			}

			if (element.HasAttribute("preProcess"))
			{
				connectedPreProcess = new Process(element.GetAttribute("preProcess"));
			}

			items = new FormItemList(this);
			FormItemFactory.MakeChildren(element.GetChild("items"), items);
		}

		public FormItemList ItemList
		{
			get { return items; }
			set { items = value; }
		}

		#region IForm Members

		public FieldList GetAllFields()
		{
			FieldList fieldList = new FieldList();

			foreach (IField field in items.RecursiveEnumerator)
			{
				if (isFormItemField(field))
				{
					fieldList.Add(field);
				}
			}

			return fieldList;
		}

		public FieldList GetMCFields()
		{
			FieldList fieldList = new FieldList();

			foreach (IFormItem item in items)
			{
				if (item is IMcqItem)
				{
					fieldList.AddUnique(item as IMcqItem);
				}
			}

			return fieldList;
		}

		public IProcess ConnectedProcess
		{
			get { return connectedProcess; }
			set { connectedProcess = value; ; }
		}

		public IProcess ConnectedPreProcess
		{
			get { return connectedPreProcess; }
			set { connectedPreProcess = value; }
		}

		public IFormItem GetFormItem(string formItemLabel)
		{
			foreach (IFormItem item in items)
			{
				if (GetDefaultLabel(item) == formItemLabel || item.AlternateLabel == formItemLabel)
				{
					return item;
				}
			}

			return null;
		}

		public IFormItem GetFormItem(int formItemId)
		{
			return items.GetItemById(formItemId);
		}

		public string GetDefaultLabel(IFormItem formItem)
		{
			return items.GetDefaultLabel(formItem);
		}

		public bool IsDataSource
		{
			get { return DataSourceName.Length > 0; }
		}

		public string DataSourceName
		{
			get;
            set;
		}

		public bool StartingPoint
		{
			get { return startPoint; }
			set { startPoint = value; }
		}

		public FieldList GetFormItemFields()
		{
			FieldList fieldList = new FieldList();

			foreach (IField field in items.RecursiveEnumerator)
			{
				if (isFormItemField(field))
				{
					fieldList.Add(field);
				}
			}

			return fieldList;
		}

		private bool isFormItemField(IField field)
		{
			return field is IMcqItem || field is IBlank || field is IHiddenField;
		}

		public FieldList GetFields()
		{
			FieldList fieldList = new FieldList();

			fieldList.Add(items);

			return fieldList;
		}

		public FieldList GetFormItemFieldsAndRecordVariables()
		{
			FieldList fieldList = new FieldList();

			foreach (IField field in items.RecursiveEnumerator)
			{
				if (isFormItemField(field))
				{
					fieldList.Add(field);
				}
			}

			foreach (IField field in getVariablesFromConnectedProcess())
			{
				fieldList.Add(field);
			}

			return fieldList;
		}

		private VariableList getVariablesFromConnectedProcess()
		{
			VariableList variables = new VariableList();

			if (connectedProcess != null)
			{
				variables = connectedProcess.Variables;
			}

			return variables;
		}

		public SkipToDestinationList SkipToDestinations
		{
			get 
			{
				if (skipToDestinations == null)
				{
					skipToDestinations = new SkipToDestinationList();
				}

				skipToDestinations.Clear();

				foreach (FormItem formItem in ItemList)
				{
					if (formItem.IsTextItem || formItem.IsQuestionItem || formItem is IHeadingItem)
					{
						skipToDestinations.Add(new SkipToDestinationItem(formItem));
					}
				}

				skipToDestinations.Add(new SkipToDestinationItem());

				return skipToDestinations;
			}
		}

		private bool dataEntryOnly = false;

		public bool DataEntryOnly
		{
			get { return dataEntryOnly; }
			set
			{
				if (dataEntryOnly != value)
				{
					dataEntryOnly = value;
					Project.Events.RaiseFormChangedEvent(new ComponentEventArgs(this));
				}
			}
		}

		public Process GetSkipInstructions(ProcessStatement statement)
		{
			foreach (IFormItem item in items)
			{
				ISkipInstructionsItem skipItem = item as ISkipInstructionsItem;

				if (skipItem != null)
				{
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

		[NonSerialized]
		private ISkipInstructionsItem activeSkipInstructionsItem = null;

		public ISkipInstructionsItem ActiveSkipToItem
		{
			get
			{
				if (!items.Contains(activeSkipInstructionsItem))
				{
					activeSkipInstructionsItem = null;
				}

				return activeSkipInstructionsItem;
			}
			set
			{
				if (!items.Contains(value))
				{
					activeSkipInstructionsItem = null;
				}
				else
				{
					activeSkipInstructionsItem = value;
				}
			}
		}

		public void ResolveProcessReferences()
		{
			if (connectedProcess != null)
			{
				connectedProcess = Project.Current.GetProcess(connectedProcess.Name);
			}

			if (connectedPreProcess != null)
			{
				connectedPreProcess = Project.Current.GetProcess(connectedPreProcess.Name);
			}
		}

		#endregion


		#region IComponent Members

		public string Name
		{
			get { return name; }
			set { name = value.Trim(); }
		}

		public string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();

			xmlString.AppendFormat(@"<form name=""{0}"" startPoint=""{1}""", name, Convert.ToBoolean(startPoint).ToString().ToLower());
            xmlString.Append(getProcessAttributeXml());

            if (dataEntryOnly)
            {
                xmlString.AppendFormat(" dataEntryOnly=\"{0}\"", dataEntryOnly.ToString().ToLower()); 
            }

            if (IsDataSource)
            {
                xmlString.AppendFormat(" dataSourceName=\"{0}\"", DataSourceName); 
            }

            xmlString.Append(">");

			xmlString.Append(items.ToXml());
			xmlString.Append(@"</form>");

			return xmlString.ToString();
		}

		private string getProcessAttributeXml()
		{
			StringBuilder attributeXml = new StringBuilder();

			if (connectedProcess != null)
			{
				attributeXml.AppendFormat(@" process=""{0}""", connectedProcess.Name);
			}

			if (connectedPreProcess != null)
			{
				attributeXml.AppendFormat(@" preProcess=""{0}""", connectedPreProcess.Name);
			}

			return attributeXml.ToString();
		}

		public string UserVisibleComponentTypeName
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		public override string ToString()
		{
			return name;
		}


        #region IFunctionParameterXml Members

        public string ToFunctionParameterXml()
        {
            return ToString();
        }

        #endregion
    }
}
