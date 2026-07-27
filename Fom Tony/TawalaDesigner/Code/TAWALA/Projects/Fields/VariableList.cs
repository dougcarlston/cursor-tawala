// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace Tawala.Projects.Fields
{
    [Serializable]
    public class VariableList : Collection<Variable>, IField
    {
        private readonly SortedList variableIndexes = new SortedList();
        protected Boolean raiseChangedEvent;

        public VariableList()
        {
        }

        public VariableList(Boolean raiseChangedEvent)
        {
            this.raiseChangedEvent = raiseChangedEvent;
        }

        #region IField Members

        public string QualifiedFieldName { get { throw new NotImplementedException(); } }

        #endregion

        /// <summary>
        /// Get the index of the Variable object with the specified name
        /// </summary>
        /// <param name="name">Name of variable</param>
        /// <returns>Index, or -1 if no match found</returns>
        //public int IndexOf(string name)
        //{
        //    for (int i = 0; i < Count; i++)
        //    {
        //        Variable var = this[i];
        //        if (var.FieldName == name)
        //        {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}
        public int IndexOf(string name)
        {
            if (variableIndexes.ContainsKey(name))
            {
                return (int)variableIndexes[name];
            }

            return -1;
        }

        protected override void InsertItem(int index, Variable item)
        {
            insertItem(index, item);

            if (raiseChangedEvent)
            {
                Project.Events.RaiseVariableChangedEvent(new VariableEventArgs(item));
            }
        }

        private void insertItem(int index, Variable item)
        {
            base.InsertItem(index, item);
            variableIndexes[getUnqualifiedName(item.FieldName)] = index;
        }

        public void AddUnique(string name)
        {
            string variableName = getUnqualifiedName(name);

            if (!inVariableList(variableName))
            {
            	bool addToFieldMapById = !Project.FieldMapByName.ContainsKey(variableName);
				add(new Variable(variableName, addToFieldMapById));
            }
        }

        public void AddUnique(Variable variable)
        {
            if (!inVariableList(variable.FieldName))
            {
                //Add(variable);
                add(variable);
            }
        }

        public new void Add(Variable item)
        {
            add(item);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            variableIndexes.Clear();
        }

        private void add(Variable variable)
        {
            base.Add(variable);
        }

        //private bool inVariableList(string variableName)
        //{
        //    return (IndexOf(variableName) >= 0);
        //}

        private bool inVariableList(string variableName)
        {
            return (variableIndexes.ContainsKey(variableName));
        }

        private string getUnqualifiedName(string name)
        {
            string unqualifiedName = name;

            if (name.Contains(":"))
            {
                int colonIndex = name.IndexOf(':') + 1;
                unqualifiedName = name.Substring(colonIndex, name.Length - colonIndex);
            }

            return unqualifiedName;
        }

        #region IField Interface

        private readonly int id = Project.NextUniqueID;

        public string FieldName { get { return ""; } }

        public string FieldString { get { return ""; } }

        public IField this[string name]
        {
            get
            {
                //foreach (IField field in this.RecursiveEnumerator)
                //{
                //    if (field.FieldName == name)
                //    {
                //        return field;
                //    }
                //}

                //// field name not found
                //return null;

                int index = IndexOf(name);

                if (index != -1)
                {
                    return this[index];
                }

                return null;
            }
        }

        public int Id { get { return id; } }

        #endregion

        #region IRecursiveEnumerable Interface

        public IEnumerable RecursiveEnumerator
        {
            get
            {
                foreach (IField item in Items)
                {
                    foreach (IField field in item)
                    {
                        yield return field;
                    }
                }
            }
        }

        #endregion
    }
}