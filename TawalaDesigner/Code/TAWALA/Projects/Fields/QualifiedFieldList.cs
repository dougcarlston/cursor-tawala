// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;

namespace Tawala.Projects
{
    [Serializable]
    public class QualifiedFieldList : IPaletteField, IOperatorDataSource, IDeserializedField
    {
        private readonly IField fields;
        protected Record qualifier;

        public QualifiedFieldList()
        {
        }

        public QualifiedFieldList(Record qualifier, IField fields)
        {
            this.qualifier = qualifier;
            this.fields = fields;
        }

        public IField Fields { get { return fields; } }

        #region IDeserializedField Members

        public IDeserializedField DeserializedFieldReference { get { return this; } }

        #endregion

        public override string ToString()
        {
            return FieldName;
        }

        #region IField Interface

        private readonly int id = Project.NextUniqueID;

        public string FieldName
        {
            get
            {
                string fieldName = "";

                foreach (IField field in fields.RecursiveEnumerator)
                {
                    fieldName = qualifier.FieldName + ":" + field.FieldName;
                }

                return fieldName;
            }
        }

        public string QualifiedFieldName { get { return FieldName; } }

        public string FieldString { get { return "<<" + FieldName + ">>"; } }

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

        public int Id { get { return id; } }

        #endregion

        #region IEnumerable Interface

        public IEnumerator GetEnumerator()
        {
            foreach (IField field in fields)
            {
                yield return new Variable(qualifier.FieldName + ":" + field.FieldName);
            }
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public IEnumerable RecursiveEnumerator
        {
            get
            {
                foreach (IPaletteField field in fields.RecursiveEnumerator)
                {
                    yield return new RecordField(qualifier, field, false);
                }
            }
        }

        #endregion

        #region IOperatorDataSource

        public IList OperatorDataSource { get { return ((IOperatorDataSource)fields).OperatorDataSource; } }

        #endregion
    }
}