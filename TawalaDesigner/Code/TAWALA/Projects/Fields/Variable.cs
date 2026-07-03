// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;

namespace Tawala.Projects.Fields
{
    [Serializable]
    public class Variable : IAssignableField, IDeserializedField
    {
        public Variable() : this("")
        {
        }

        public Variable(string name) : this(name, true)
        {
        }

        public Variable(string name, bool addToFieldMap)
        {
            fieldName = name.Trim();

            if (addToFieldMap)
            {
                Project.FieldMapById.AddUnique(this);
            }

            Project.FieldMapByName.AddUnique(this);
        }

        #region IAssignableField Members

        public string AssignmentName { get { return FieldName; } }

        #endregion

        #region IDeserializedField Members

        public IDeserializedField DeserializedFieldReference { get { return (IDeserializedField)Project.FieldMapById[Id]; } }

        #endregion

        public override string ToString()
        {
            return QualifiedFieldName;
        }

        public string GetToolTipText()
        {
            string processNames = string.Empty;

            foreach (Process proc in getProcessesForVariable())
            {
                if (processNames.Length > 0)
                {
                    processNames += ", ";
                }

                processNames += proc.Name;
            }

            return processNames;
        }

        private Collection<Process> getProcessesForVariable()
        {
            var processes = new Collection<Process>();

            foreach (Process proc in Project.Current.ProcessList)
            {
                if (proc.Variables.IndexOf(fieldName) >= 0)
                {
                    processes.Add(proc);
                }
            }

            foreach (IForm form in Project.Current.FormList)
            {
                foreach (IFormItem item in form.ItemList)
                {
                    if (item is SkipInstructionsItem)
                    {
                        Process proc = ((SkipInstructionsItem)item).Instructions;

                        if (proc.Variables.IndexOf(fieldName) >= 0)
                        {
                            processes.Add(proc);
                        }
                    }
                }
            }
            return processes;
        }

        #region IField Interface

        private readonly int id = Project.NextUniqueID;
        protected string fieldName = "";

        public string FieldName { get { return fieldName; } set { fieldName = value; } }

        public virtual string QualifiedFieldName { get { return fieldName; } }

        public string FieldString { get { return "<<" + fieldName + ">>"; } }

        public int Id { get { return id; } }

        #endregion

        #region IEnumerable Interface

        public IEnumerator GetEnumerator()
        {
            yield return this;
        }

        public IField this[string name]
        {
            get
            {
                if (FieldName == name)
                {
                    return this;
                }

                return null;
            }
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public IEnumerable RecursiveEnumerator { get { yield return this; } }

        #endregion

        #region IOperatorDataSource Interface

        public IList OperatorDataSource { get { return HybridOperator.List.DataSource; } }

        #endregion
    }
}