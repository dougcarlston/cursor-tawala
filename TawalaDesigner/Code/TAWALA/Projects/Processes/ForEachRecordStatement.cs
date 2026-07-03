// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Text;
using Tawala.Projects.Fields;
using Tawala.XmlSupport;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements a ForEach statement in the Process
    /// </summary>
    [Serializable]
    public sealed class ForEachRecordStatement : ForEachStatement
    {
        private const string xmlForEachStartTag = "<foreach record=\"$RECORDNAME\" recordList=\"$RECORDLISTNAME\">";

        [NonSerialized]
        private Record record;

        private SerializationInfo serializationInfo;

        public ForEachRecordStatement()
        {
        }

        public ForEachRecordStatement(Record record, RecordSet recordSet) : this()
        {
            this.record = record;
            RecordList = recordSet;
        }

        /// <summary>
        /// Construct ForEachRecordStatement from XML element (e.g. "<foreach record="Record" recordList="Record Set">")
        /// </summary>
        public ForEachRecordStatement(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
        {
        }

        public ForEachRecordStatement(IXmlElement element, Process process) : this()
        {
            record = new Record(element.GetAttribute("record"));
            process.Records.AddUnique(record);

            RecordList = (RecordSet)process.RecordSets[element.GetAttribute("recordList")];

            mapFields(process);
            EnclosedStatements = new ProcessStatementList(element, process);
        }

        public Record Record
        {
            set { record = value; }
            get { return record; }
        }

        public RecordSet RecordList { get; set; }

        public override string Qualifier
        {
            get { return record.FieldName; }
        }

        /// <summary>
        /// Adds record-qualified fields (such as "Record:Q1:a") to the specified process's field mapper.
        /// </summary>
        private void mapFields(Process process)
        {
            process.MappedFields.Qualifiers.Add(record.FieldName);

            RecordList.MapFormFields(process);

            process.MappedFields.Map();
        }

        public override Record GetRecord()
        {
            return record;
        }

        public override RecordSet GetRecordSet()
        {
            return RecordList;
        }

        /// <summary>
        /// Returns a list of qualified fields (such as "Record:Q1:a" or "Record:Q2") for this statement and
        /// all statements enclosing it.
        /// </summary>
        public override IField GetQualifiedFields(Process process)
        {
            var fields = new FieldList();

            foreach (IForm form in RecordList.Forms)
            {
                fields.Add(new QualifiedFieldList(record, form.GetFormItemFieldsAndRecordVariables()));
            }

            int index = process.Lines.GetIndex(this);
            var enclosingForEachStatement = (ForEachStatement)process.Lines.GetEnclosingForEachStatement(index);

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
        public override IField GetQualifiedMcFields(Process process)
        {
            var fields = new FieldList();

            foreach (IForm form in RecordList.Forms)
            {
                fields.Add(new QualifiedFieldList(record, form.GetMCFields()));
            }

            int index = process.Lines.GetIndex(this);
            var enclosingForEachStatement = (ForEachStatement)process.Lines.GetEnclosingForEachStatement(index);

            if (enclosingForEachStatement != null)
            {
                fields.Add(enclosingForEachStatement.GetQualifiedMcFields(process));
            }

            return fields;
        }

        public override IField GetQualifiedVariables(Process process)
        {
            var fields = new FieldList();

            fields.Add(new QualifiedFieldList(record, process.Variables));

            return fields;
        }

        /// <summary>
        /// Returns a list of record names corresponding to this statement and the FOR EACH statements enclosing this statement in the specified process.
        /// </summary>
        public override IList GetRecordNames(Process process)
        {
            var recordNames = new ArrayList
                              {
                                  record.FieldName
                              };

            int index = process.Lines.GetIndex(this);
            var enclosingForEachStatement = (ForEachStatement)process.Lines.GetEnclosingForEachStatement(index);

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
            var records = new FieldList();

            records.Add(record);

            int index = process.Lines.GetIndex(this);
            var enclosingForEachStatement = (ForEachStatement)process.Lines.GetEnclosingForEachStatement(index);

            if (enclosingForEachStatement != null)
            {
                foreach (IField r in enclosingForEachStatement.GetRecords(process))
                {
                    records.Add(r);
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
            return Name + " " + record.FieldName + " in " + RecordList.FieldName;
        }

        public override string ToXml()
        {
            var xmlString = new StringBuilder(xmlForEachStartTag);

            xmlString.Replace("$RECORDNAME", record.FieldName);
            xmlString.Replace("$RECORDLISTNAME", RecordList.FieldName);

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

        #region Nested type: SerializationInfo

        [Serializable]
        private sealed class SerializationInfo
        {
            private readonly string recordName;

            public SerializationInfo(ForEachRecordStatement statement)
            {
                recordName = statement.Record.FieldName;
            }

            public void Deserialized(ForEachRecordStatement statement)
            {
                statement.Record = new Record(recordName);
            }
        }

        #endregion
    }
}