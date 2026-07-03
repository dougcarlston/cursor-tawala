// $Workfile: ForEachStatement.cs $
// $Revision: 26 $	$Date: 5/31/07 2:48p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements a base ForEach statement in the Process
	/// </summary>
	[Serializable]
	public class ForEachStatement : Tawala.Projects.ProcessStatement
	{
		protected ProcessStatementList enclosedStatements = new ProcessStatementList();

		public ProcessStatementList EnclosedStatements
		{
			get
			{
				return enclosedStatements;
			}
		}

		public ForEachStatement()
		{
			name = "For Each";
		}

		public override string ToString()
		{
			return Name + " ?????"; // temporary, function will be removed later
		}

		public override string ToXml()
		{
			throw new InvalidOperationException("Only derived version should be called!");
		}

		public virtual string Qualifier
		{
			get
			{
				return "";
			}
		}

		public virtual IField GetQualifiedFields(Process process)
		{
			return FieldList.NULL;
		}

		public virtual IField GetQualifiedMCFields(Process process)
		{
			return FieldList.NULL;
		}

		public virtual Record GetRecord()
		{
			return null;
		}

		public virtual RecordSet GetRecordSet()
		{
			return null;
		}

		public virtual IField GetUnqualifiedFields()
		{
			return FieldList.NULL;
		}

		public virtual IField GetUnqualifiedMCFields()
		{
			return FieldList.NULL;
		}

		public virtual IField GetQualifiedVariables(Process process)
		{
			return FieldList.NULL;
		}

		public virtual IList GetRecordNames(Process process)
		{
			return new ArrayList();
		}

		public virtual IField GetRecords(Process process)
		{
			return FieldList.NULL;
		}

	}

	/// <summary>
	/// Implements a ForEach statement in the Process
	/// </summary>
	[Serializable]
	public class ForEachRecordStatement : ForEachStatement
	{
		public ForEachRecordStatement() : base()
		{
		}

		public ForEachRecordStatement(Record record, RecordSet recordSet) : this()
		{
			this.record = record;
			this.recordSet = recordSet;
		}

		/// <summary>
		/// Construct ForEachRecordStatement from XML element (e.g. "<foreach record="Record" recordList="Record Set">")
		/// </summary>
		public ForEachRecordStatement(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
		{
		}

		public ForEachRecordStatement(IXmlElement element, Process process) : this()
		{
			this.record = new Record(element.GetAttribute("record"));
			process.Records.AddUnique(record);

			this.recordSet = (RecordSet)process.RecordSets[element.GetAttribute("recordList")];

			mapFields(process);
			this.enclosedStatements = new ProcessStatementList(element, process);
		}

		/// <summary>
		/// Adds record-qualified fields (such as "Record:Q1:a") to the specified process's field mapper.
		/// </summary>
		private void mapFields(Process process)
		{
			process.MappedFields.Qualifiers.Add(record.FieldName);

			recordSet.MapFormFields(process);

			process.MappedFields.Map();
		}

		[NonSerialized]
		private Record record;

		public Record Record
		{
			set
			{
				record = value;
			}
			get
			{
				return record;
			}
		}

		private RecordSet recordSet;

		public RecordSet RecordList
		{
			set
			{
				recordSet = value;
			}
			get
			{
				return recordSet;
			}
		}

		public override string Qualifier
		{
			get
			{
				return record.FieldName;
			}
		}

		public override Record GetRecord()
		{
			return record;
		}

		public override RecordSet GetRecordSet()
		{
			return recordSet;
		}

		/// <summary>
		/// Returns a list of qualified fields (such as "Record:Q1:a" or "Record:Q2") for this statement and
		/// all statements enclosing it.
		/// </summary>
		public override IField GetQualifiedFields(Process process)
		{
			FieldList fields = new FieldList();

			foreach (IForm form in recordSet.Forms)
			{
				fields.Add(new QualifiedFieldList(record, form.GetFormItemFieldsAndRecordVariables()));
			}

			int index = process.Lines.GetIndex(this);
			ForEachStatement enclosingForEachStatement = (ForEachStatement)process.Lines.GetEnclosingForEachStatement(index);

			if (enclosingForEachStatement != null)
			{
				fields.Add(enclosingForEachStatement.GetQualifiedFields(process));
			}

			return fields;
		}

		/// <summary>
		/// Returns a list of qualified MC fields (such as "Record:Q1") for this statement and
		/// all statements enclosing it.
		/// </summary>
		public override IField GetQualifiedMCFields(Process process)
		{
			FieldList fields = new FieldList();

			foreach (IForm form in recordSet.Forms)
			{
				fields.Add(new QualifiedFieldList(record, form.GetMCFields()));
			}

			int index = process.Lines.GetIndex(this);
			ForEachStatement enclosingForEachStatement = (ForEachStatement)process.Lines.GetEnclosingForEachStatement(index);

			if (enclosingForEachStatement != null)
			{
				fields.Add(enclosingForEachStatement.GetQualifiedMCFields(process));
			}

			return fields;
		}

		//public override IField GetQualifiedVariables()
		//{
		//    FieldList fields = new FieldList();

		//    if (recordSet.Forms.Count > 0)
		//    {
		//        fields.Add(new QualifiedFieldList(record, recordSet.Forms[0].RecordVariables));
		//    }

		//    return fields;
		//}

		public override IField GetQualifiedVariables(Process process)
		{
			FieldList fields = new FieldList();

			fields.Add(new QualifiedFieldList(record, process.Variables));

			return fields;
		}


		/// <summary>
		/// Returns a list of record names corresponding to this statement and the FOR EACH statements enclosing this statement in the specified process.
		/// </summary>
		public override IList GetRecordNames(Process process)
		{
			ArrayList recordNames = new ArrayList();

			recordNames.Add(record.FieldName);

			int index = process.Lines.GetIndex(this);
			ForEachStatement enclosingForEachStatement = (ForEachStatement)process.Lines.GetEnclosingForEachStatement(index);

			if (enclosingForEachStatement != null)
			{
				foreach (string recordName in enclosingForEachStatement.GetRecordNames(process))
				{
					recordNames.Add(recordName);
				}
			}

			return recordNames;
		}

		/// <summary>
		/// Returns a list of records corresponding to this statement and the FOR EACH statements enclosing this statement in the specified process.
		/// </summary>
		public override IField GetRecords(Process process)
		{
			FieldList records = new FieldList();

			records.Add(this.record);

			int index = process.Lines.GetIndex(this);
			ForEachStatement enclosingForEachStatement = (ForEachStatement)process.Lines.GetEnclosingForEachStatement(index);

			if (enclosingForEachStatement != null)
			{
				foreach (IField record in enclosingForEachStatement.GetRecords(process))
				{
					records.Add(record);
				}
			}

			return records;
		}

		public override Type GetStatementType()
		{
			return typeof(ForEachStatement);
		}

		public override string ToString()
		{
			return Name + " " + record.FieldName + " in " + recordSet.FieldName;
		}

		private static readonly string xmlForEachStartTag = "<foreach record=\"$RECORDNAME\" recordList=\"$RECORDLISTNAME\">";

		public override string ToXml()
		{
			StringBuilder xmlString = new StringBuilder(xmlForEachStartTag);

			xmlString.Replace("$RECORDNAME", record.FieldName);
			xmlString.Replace("$RECORDLISTNAME", recordSet.FieldName);

			return xmlString.ToString();
		}

		[OnSerializing]
		private void onSerializing(StreamingContext context)
		{
			serializationInfo = new SerializationInfo(this);
		}

		[OnDeserialized]
		private void onDeserialized(StreamingContext context)
		{
			serializationInfo.Deserialized(this);
			serializationInfo = null;
		}

		private SerializationInfo serializationInfo = null;

		[Serializable]
		class SerializationInfo
		{
			private string recordName;

			public SerializationInfo(ForEachRecordStatement statement)
			{
				recordName = statement.Record.FieldName;
			}

			public void Deserialized(ForEachRecordStatement statement)
			{
				statement.Record = new Record(recordName);
			}
		}
	}


	#region ForEachQuestionStatement currently unused
#if false
	/// <summary>
	/// Implements a ForEachQuestion statement in the Process
	/// </summary>
	[Serializable]
	public class ForEachQuestionStatement : ForEachStatement
	{
		public ForEachQuestionStatement() : base()
		{
		}

		/// <summary>
		/// Construct ForEachQuestionStatement from XML element (e.g. "<forEachMc>")
		/// </summary>
		public ForEachQuestionStatement(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
		{
		}

		public ForEachQuestionStatement(IXmlElement element, Process process) : this()
		{
			mapFields(process);
			this.enclosedStatements = new ProcessStatementList(element, process);
		}

		/// <summary>
		/// Adds pertinent fields (such as "Record:(selection)") to the field mapper.
		/// </summary>
		private void mapFields(Process process)
		{
			process.MappedFields.Fields.AddUnique(new Field("(selection)"));
			process.MappedFields.Map();
		}

		/// <summary>
		/// Returns a list of qualified fields (such as "Record:Q1:a") for this statement and
		/// all statements enclosing it.
		/// </summary>
		public override IField GetQualifiedFields(Process process)
		{
			FieldList fields = new FieldList();

			int index = process.Lines.GetIndex(this);

			ProcessStatementList enclosingForEachStatements = process.Lines.GetEnclosingForEachStatements(index);

			foreach (ProcessStatement statement in enclosingForEachStatements)
			{
				ForEachStatement enclosingForEachStatement = statement as ForEachStatement;

				if (enclosingForEachStatement != null)
				{
					fields.Add(enclosingForEachStatement.GetQualifiedFields(process));

					foreach (Form form in enclosingForEachStatement.GetRecordSet().Forms)
					{
						fields.Add(new QualifiedFieldList(enclosingForEachStatement.GetRecord(), new MCItemProxy()));
					}
				}
			}

			return fields;
		}

		/// <summary>
		/// Returns a list of qualified MC fields (such as "Record:Q1") for this statement and
		/// all statements enclosing it.
		/// </summary>
		public override IField GetQualifiedMCFields(Process process)
		{
			FieldList fields = new FieldList();

			int index = process.Lines.GetIndex(this);

			ProcessStatementList enclosingForEachStatements = process.Lines.GetEnclosingForEachStatements(index);

			foreach (ProcessStatement statement in enclosingForEachStatements)
			{
				ForEachStatement enclosingForEachStatement = statement as ForEachStatement;

				if (enclosingForEachStatement != null)
				{
					fields.Add(enclosingForEachStatement.GetQualifiedMCFields(process));

					foreach (Form form in enclosingForEachStatement.GetRecordSet().Forms)
					{
						fields.Add(new QualifiedFieldList(enclosingForEachStatement.GetRecord(), new MCItemProxy()));
					}
				}
			}

			return fields;
		}

		public override IField GetUnqualifiedFields()
		{
			return GetUnqualifiedMCFields();
		}

		public override IField GetUnqualifiedMCFields()
		{
			return (new MCItemProxy());
		}

		/// <summary>
		/// Returns a list of record names (such as "Record") for this statement and
		/// all statements enclosing it.
		/// </summary>
		public override IList GetRecordNames(Process process)
		{
			ArrayList recordNames = new ArrayList();

			int index = process.Lines.GetIndex(this);
			ForEachStatement enclosingForEachStatement = (ForEachStatement)process.Lines.GetEnclosingForEachStatement(index);

			if (enclosingForEachStatement != null)
			{
				foreach (string recordName in enclosingForEachStatement.GetRecordNames(process))
				{
					recordNames.Add(recordName);
				}
			}

			return recordNames;
		}


		public override Type GetStatementType()
		{
			return typeof(ForEachStatement);
		}

		public override string ToString()
		{
			// REVISIT: string should come from somewhere else but this will do for now to make sure we are working with the right class.
			return Name + " Multiple Choice Question"; 
		}

		private static readonly string xmlForEachStartTag = "<forEachMc>";

		public override string ToXml()
		{
			StringBuilder xmlString = new StringBuilder(xmlForEachStartTag);

			return xmlString.ToString();
		}
	}
#endif
	#endregion
}
