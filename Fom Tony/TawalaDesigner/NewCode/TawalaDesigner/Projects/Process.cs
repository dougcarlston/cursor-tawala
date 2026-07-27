// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using Tawala.Common;
using Tawala.Projects.Forms;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	[Serializable]
	public class Process : Component, IProcess
	{
		public static Process NULL = new NullProcess("Null Process");
		protected static Factory<ProcessStatement> statementFactory = new Factory<ProcessStatement>();

		static Process()
		{
			statementFactory.Register("show", "document", typeof(ShowDocumentStatement));
			statementFactory.Register("show", "form", typeof(ShowFormStatement));
			statementFactory.Register("show", typeof(ShowUrlStatement));
			statementFactory.Register("edit", typeof(ShowRecordStatement));
			statementFactory.Register("if", typeof(IfStatement));
			statementFactory.Register("set", typeof(SetStatement));
			statementFactory.Register("send", typeof(SendStatement));

			statementFactory.Register("addTo", typeof(AddStatement));
			statementFactory.Register("subtractFrom", typeof(SubtractStatement));
			statementFactory.Register("multiplyBy", typeof(MultiplyStatement));
			statementFactory.Register("divideBy", typeof(DivideStatement));

			statementFactory.Register("append", typeof(AppendStatement));

			statementFactory.Register("get", typeof(GetStatement));
			statementFactory.Register("foreach", typeof(ForEachRecordStatement));
			statementFactory.Register("delete", typeof(DeleteStatement));
			statementFactory.Register("comment", typeof(CommentStatement));

			statementFactory.Register("skip", typeof(SkipToStatement));
		}

		public Process(string name) : base(name)
		{
			Project.Events.StatementAdded += events_LineListChanged;
			Project.Events.StatementModified += events_LineListChanged;
			Project.Events.StatementRemoved += events_LineListChanged;

			Project.Events.FormItemAdded += events_FormItemChanged;
			Project.Events.FormItemChanged += events_FormItemChanged;
			Project.Events.FormItemRemoved += events_FormItemChanged;
			Project.Events.FormChanged += events_FormChanged;

			lines = new ProcessLineList(this);
		}

		public Process()
		{
			Project.Events.StatementAdded += events_LineListChanged;
			Project.Events.StatementModified += events_LineListChanged;
			Project.Events.StatementRemoved += events_LineListChanged;

			Project.Events.FormItemAdded += events_FormItemChanged;
			Project.Events.FormItemChanged += events_FormItemChanged;
			Project.Events.FormItemRemoved += events_FormItemChanged;
			Project.Events.FormChanged += events_FormChanged;

			lines = new ProcessLineList(this);
		}

		void events_FormItemChanged(object sender, FormItemEventArgs e)
		{
			baseFieldsAreCurrent = false;
			qualifiedFieldsCacheManager.Reset();
		}

		void events_FormChanged(object sender, ComponentEventArgs e)
		{
			baseFieldsAreCurrent = false;
			qualifiedFieldsCacheManager.Reset();
		}


		public Process(IXmlElement element) : this(element.GetAttribute("name") ?? string.Empty)
		{
			foreach (IXmlElement childElement in element.GetChildren())
			{
				ProcessStatement statement = statementFactory.MakeObject(childElement, this);
				lines.Add(new ProcessLineList(statement));
			}
		}

        public override string UserVisibleComponentTypeName
        {
            get
            {
                return Properties.Resources.ProcessComponentVisibleTypeName;
            }
        }

		public void MapFormFields(IForm form)
		{
			foreach (IField field in form.GetAllFields().RecursiveEnumerator)
			{
				if (field.FieldName != "")
				{
					MappedFields.Fields.Add(field);
				}
			}
		}

        void events_LineListChanged(object sender, StatementEventArgs e)
		{
			if (e.Process == this && Lines.EnableEvents == true)
			{
				if (statementSetsVariable(e.Statement))
				{
					updateProcessVariables();
				}
				else if (e.Statement is GetStatement)
				{
					updateRecordSets();
				}
			}

			baseFieldsAreCurrent = false;
			qualifiedFieldsCacheManager.Reset();
		}

		private static bool statementSetsVariable(ProcessStatement statement)
		{
			return (statement is SetStatement || statement is ArithmeticStatement);
		}

		private static string getVariableName(ProcessStatement statement)
		{
			string variableName = "";
			if (statement is SetStatement)
			{
				variableName = ((SetStatement)statement).Variable.FieldName;
			}
			else if (statement is ArithmeticStatement)
			{
				variableName = ((ArithmeticStatement)statement).Variable;
			}
			return variableName;
		}

		private void updateProcessVariables()
		{
			variables.Clear();

			foreach (ProcessLine line in lines)
			{
				if (line is SetLine)
				{
					if (((SetStatement)line.Statement).Variable is Variable)
					{
						variables.AddUnique(((SetStatement)line.Statement).Variable as Variable);
					}
				}
				else if (line is ArithmeticLine)
				{
					if (isVariable(((ArithmeticStatement)line.Statement).Variable))
					{
						variables.AddUnique(((ArithmeticStatement)line.Statement).Variable);
					}
				}
			}
			Project.Events.RaiseProcessVariableListChangedEvent(new ComponentEventArgs(this));
		}

		private void updateRecordSets()
		{
			recordSets.Clear();

			foreach (ProcessLine line in lines)
			{
				if (line is GetLine)
				{
					recordSets.AddUnique(((GetStatement)line.Statement).Records);
				}
			}
		}

		private static bool isVariable(string fieldString)
		{
			return FieldUtil.IsVariable(fieldString);
		}

		private static FormList getFormListForRecordSet(ProcessStatement statement, Process process)
		{
			FormList forms = new FormList();

			int index = process.Lines.GetIndex(statement);
			if (index != -1)
			{
				ForEachStatement forEachStatement = (ForEachStatement)process.Lines.GetEnclosingForEachStatement(index);
				if (forEachStatement != null)
				{
					forms = forEachStatement.GetRecordSet().Forms;
				}
			}

			return forms;
		}

		// XML for a Process component
		private const string xmlProcessStartTag = "<process name=\"$PROCNAME\">\r\n";
		private const string xmlProcessEndTag = "</process>\r\n";

		public override string ToXml()
		{
			// start with placeholder string
			StringBuilder xmlString = new StringBuilder(xmlProcessStartTag);

			// replace process name placeholder with actual value
			xmlString.Replace("$PROCNAME", XMLStringFormatter.EscapeAttributeText(Name));

			// append process line list xml
			xmlString.Append(lines.ToXml());

			// append process end tag
			xmlString.Append(xmlProcessEndTag);

			return xmlString.ToString();
		}

		/// <summary>
		/// Process lines. Data source for Process window.
		/// </summary>
		private ProcessLineList lines;

		public ProcessLineList Lines
		{
			get
			{
				return lines;
			}
			set
			{
				lines = value;
			}
		}

		// list of variables
		private VariableList variables = new VariableList(true);

		public VariableList Variables
		{
			get
			{
				return variables;
			}
			set
			{
				variables = value;
			}
		}

		/// <summary>
		/// List of record variables.
		/// </summary>
		[OptionalField()]
		private VariableList records = new VariableList();

		public VariableList Records
		{
			get
			{
				if (records == null)
				{
					records = new VariableList();
				}
				return records;
			}
			set
			{
				records = value;
			}
		}

		/// <summary>
		/// List of record set variables.
		/// </summary>
		[OptionalField()]
		private VariableList recordSets = new VariableList();

		public VariableList RecordSets
		{
			get
			{
				if (recordSets == null)
				{
					recordSets = new VariableList();
				}
				return recordSets;
			}
			set
			{
				recordSets = value;
			}
		}

		/// <summary>
		/// Maps field strings to field references
		/// </summary>
		
		[NonSerialized]
		private FieldMapper mappedFields;

		public FieldMapper MappedFields
		{
			get
			{
				if (mappedFields == null)
				{
					mappedFields = new FieldMapper(this.Name);
				}

				return mappedFields;
			}
		}

		/// <summary>
		/// Returns a list of fields valid for this process. Contents of the list are based on
		/// position within the Process, as specified by processLineIndex.
		/// </summary>
		public FieldList GetValidFields(int processLineIndex)
		{
			return GetValidFields((ForEachStatement)lines.GetEnclosingForEachStatement(processLineIndex));
		}

		private FieldList baseFields;
		private bool baseFieldsAreCurrent = false;

		public FieldList GetValidFields(ForEachStatement enclosingForEachStatement)
		{
			FieldList fieldList = new FieldList();

			if (!baseFieldsAreCurrent)
			{
				baseFields = getFieldList();
				baseFieldsAreCurrent = true;
			}

			fieldList.Add(baseFields);

			if (enclosingForEachStatement != null)
			{
				fieldList.Add(qualifiedFieldsCacheManager.FetchObject(enclosingForEachStatement, this));
			}

			return fieldList;
		}

		[NonSerialized]
		private QualifiedFieldsCacheManager qualifiedFieldsCacheManager = new QualifiedFieldsCacheManager();

		private class QualifiedFieldsCacheManager
		{
			private static ForEachStatementComparer comparer = new ForEachStatementComparer();
			private SortedDictionary<ForEachStatement, IField> fieldsCache = new SortedDictionary<ForEachStatement, IField>(comparer);

			public IField FetchObject(ForEachStatement forEachStatement, Process process)
			{
				if (fieldsCache.ContainsKey(forEachStatement))
				{
					return fieldsCache[forEachStatement];
				}

				IField qualifiedFields = forEachStatement.GetQualifiedFields(process);

				fieldsCache[forEachStatement] = qualifiedFields;

				return qualifiedFields;
			}

			public void Reset()
			{
				fieldsCache.Clear();
			}
		}

		private class ForEachStatementComparer : Comparer<ForEachStatement>
		{
			public override int Compare(ForEachStatement x, ForEachStatement y)
			{
				return x.ToString().CompareTo(y.ToString());
			}
		}

		/// <summary>
		/// Returns a list of MC fields valid for this process. Contents of the list are based on
		/// position within the Process, as specified by processLineIndex.
		/// </summary>
		public FieldList GetValidMCFields(int processLineIndex)
		{
			return GetValidMCFields((ForEachStatement)lines.GetEnclosingForEachStatement(processLineIndex));
		}

		public FieldList GetValidMCFields(ForEachStatement enclosingForEachStatement)
		{
			FieldList fieldList = GetMCFields();

			if (enclosingForEachStatement != null)
			{
				fieldList.AddUnique(enclosingForEachStatement.GetUnqualifiedMCFields());
				fieldList.AddUnique(enclosingForEachStatement.GetQualifiedMCFields(this));
			}

			return fieldList;
		}

		/// <summary>
		/// Returns a list of MC fields valid for this Process. Contents of the list are based on
		/// "connectedness" of this Process to Forms in the Project.
		/// </summary>
		private FieldList GetMCFields()
		{
			FieldList fieldList = new FieldList();

			foreach (IField field in getFieldList().RecursiveEnumerator)
			{
				if (field is IMcqItem)
				{
					fieldList.Add(field);
				}
			}

			return fieldList;
		}

		/// <summary>
		/// Repopulate the master field list with data from
		/// FIB items, MC items and variables.
		/// </summary>
		private FieldList getFieldList()
		{
			FieldList fieldList = new FieldList();

			FieldList fibList = new FieldList();
			FieldList mcList = new FieldList();

			for (int i = 0; i < Project.Current.FormList.Count; i++)
			{
				IForm form = Project.Current.FormList[i];

				if (this is ISkipInstructions)
				{
					for (int j = 0; j < form.ItemList.Count; ++j)
					{
						ISkipInstructionsItem item = form.ItemList[j] as ISkipInstructionsItem;

						if (item != null && item.Instructions == this)
						{
							int stopPosition = form.ItemList.Count;

							// if there's an active SkipInstructionsItem (and there better be!)
							if (form.ActiveSkipToItem != null)
							{
								Debug.Assert(form.ActiveSkipToItem == item);
								// we want only fields that preceed the skip item
								stopPosition = form.ItemList.IndexOf(form.ActiveSkipToItem);
							}

							updateFibAndMcLists(fibList, mcList, form, stopPosition);
							break;
						}
					}
				}
				else
				{
					updateFibAndMcLists(fibList, mcList, form, form.ItemList.Count);
				}

				fieldList.Add(fibList);
				fieldList.Add(mcList);
				//fieldList.Sort();

				fieldList.Add(Project.Current.AllVariables);
			}

			return fieldList;
		}

		/// <summary>
		/// Update the fibList and mcList portions of the overall fieldList
		/// </summary>
		/// <param name="fibList"> List of FIB Fields </param>
		/// <param name="mcList"> List of MCQ Fields </param>
		/// <param name="form">	The Form to be searched for Fields </param>
		/// <param name="stopPosition">
		///		The last position in the list to use for collating the list.
		///		For Process components this is the position of the last item
		///		in the entire Form.
		///		For Skip Instructions this is the position of the SkipToItem
		///		associated with the instructions. This means that only Fields
		///		preceeding the Skip can be referenced in conditionals and other
		///		statements within the instructions.
		///	</param>
		private static void updateFibAndMcLists(FieldList fibList, FieldList mcList, IForm form, int stopPosition)
		{
			if (form != null)
			{
				for (int i = 0; i < stopPosition; i++)
				{
					FormItem item = (FormItem)form.ItemList[i];

					if (item is IFibItem)
					{
						Boolean blankHasAlternateLabel = false;

						for (int j = 0; j < ((IFibItem)item).BlankList.Count; j++)
						{
							if (((IFibItem)item).BlankList[j].AlternateLabel != "")
							{
								blankHasAlternateLabel = true;
							}
						}

						addItemToList(fibList, item, blankHasAlternateLabel);
					}

					if (item is IHiddenField)
					{
						addItemToList(fibList, item, true);
					}

					if (item is IMcqItem)
					{
						addItemToList(mcList, item, ((IMcqItem)item).AlternateLabel != "");
					}
				}
			}
		}

		private static void addItemToList(FieldList list, IFormItem item, Boolean hasAlternateLabel)
		{
			if (hasAlternateLabel)
			{
				list.AddUnique(item);
			}
			else
			{
				list.Add(item);
			}
		}

		/// <summary>
		/// Returns a list of names of Record variables. The list includes the Record associated with
		/// the specified FOR EACH statement and all enclosing FOR EACH statements.
		/// </summary>
		private IList getRecordNames(int processLineIndex)
		{
			ForEachStatement forEachStatement = (ForEachStatement)lines.GetEnclosingForEachStatement(processLineIndex);
			return getRecordNames(forEachStatement);
		}

		private IList getRecordNames(ForEachStatement enclosingForEachStatement)
		{
			return (enclosingForEachStatement == null ?  new ArrayList() : enclosingForEachStatement.GetRecordNames(this));
		}

		public IList GetRecordNames(int processLineIndex)
		{
			return getRecordNames(processLineIndex);
		}

		/// <summary>
		/// Returns a list of records corresponding to the FOR EACH statements enclosing the specified process line.
		/// </summary>
		public IField GetForEachRecords(int processLineIndex)
		{
			ForEachStatement forEachStatement = (ForEachStatement)lines.GetEnclosingForEachStatement(processLineIndex);

			return (forEachStatement == null ? FieldList.NULL : forEachStatement.GetRecords(this));
		}

		/// <summary>
		/// Returns the record set corresponding to the specified record.
		/// </summary>
		public RecordSet GetRecordSet(Record record)
		{
			foreach (ProcessLine line in lines)
			{
				if (line is ForEachRecordLine)
				{
					ForEachRecordStatement forEachStatement = (ForEachRecordStatement)line.Statement;

					if (forEachStatement.Record == record)
					{
						return forEachStatement.RecordList;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Determines whether a name is valid for use as a Process variable
		/// </summary>
		public bool ValidVariableName(string name, int processLineIndex)
		{
			bool validName = true;

			string testName = name.Trim();
			string fieldName = "";

			// check qualified names
			if (testName.Contains(":"))
			{
				validName = false;

				string recordName = FieldUtil.GetRecordName(testName);
				fieldName = FieldUtil.GetFieldName(testName);

				// check for valid qualifier
				IList recordNames = getRecordNames(processLineIndex);
				if (recordNames.Contains(recordName) && fieldName.Length > 0 && ValidVariableName(fieldName))
				{
					validName = true;
				}
			}
			else
			{
				// check validity of base name
				validName = ValidVariableName(testName);
			}

			return validName;
		}

        public bool ValidRecordVariableName(string name)
        {
            string testName = name.Trim();

            if (isExistingRecordVariable(testName))
                return true;

            return ValidVariableName(testName);
        }

		public bool ValidVariableName(string name)
		{
			string testName = name.Trim();

			// first, make sure that the proposed label does not duplicate an existing Form item label
			bool validName = !Project.Current.AlternateLabelExists(testName);

			if (validName)
			{
				// check to see if it duplicates a record set name
				foreach (Variable set in RecordSets)
				{
					if (set.FieldName == testName)
					{
						validName = false;
						break;
					}
				}
			}

			if (validName)
		    {
                validName = !isExistingRecordVariable(testName);
			}

			// now check for illegal name formats
			if (validName && testName.Length > 0)
			{
				validName = Project.Current.ValidFieldLabelFormat(testName);
			}

			return validName;
		}

        private bool isExistingRecordVariable(string name)
        {
            foreach (Variable rec in Records)
            {
                if (rec.FieldName == name)
                {
                    return true; 
                }
            }

            return false;
        }

		private GetStatement activeGetStatement = null;

		public GetStatement ActiveGetStatement
		{
			get { return activeGetStatement; }
			set { activeGetStatement = value; }
		}
		
		/// <summary>
		/// Called while deserializing the object.
		/// </summary>
		[OnDeserializing]
		private void onDeserializing(StreamingContext context)
		{
			if (variables == null)
			{
				variables = new VariableList(true);
			}

			if (qualifiedFieldsCacheManager == null)
			{
				qualifiedFieldsCacheManager = new QualifiedFieldsCacheManager();
			}
		}

		/// <summary>
		/// Called after deserializing the object.
		/// </summary>
		[OnDeserialized]
		private void onDeserialized(StreamingContext context)
		{
			if (variables == null)
			{
				variables = new VariableList(true);
			}

			updateProcessVariables();
			updateRecordSets();
		}

		[Serializable]
		private class NullProcess : Process
		{
			public NullProcess(string name)	: base(name)
			{
			}
		}

	}
}
