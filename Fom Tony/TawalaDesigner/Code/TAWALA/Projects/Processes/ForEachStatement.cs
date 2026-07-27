// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements a base ForEach statement in the Process
    /// </summary>
    [Serializable]
    public class ForEachStatement : ProcessStatement
    {
        public ForEachStatement()
        {
            EnclosedStatements = new ProcessStatementList();
            name = "For Each";
        }

        public ProcessStatementList EnclosedStatements { get; protected set; }

        public virtual string Qualifier
        {
            get { return ""; }
        }

        public override string ToString()
        {
            return Name + " ?????"; // temporary, function will be removed later
        }

        public override string ToXml()
        {
            throw new InvalidOperationException("Only derived version should be called!");
        }

        public virtual IField GetQualifiedFields(Process process)
        {
            return FieldList.NULL;
        }

        public virtual IField GetQualifiedMcFields(Process process)
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

        public virtual IField GetUnqualifiedMcFields()
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
}