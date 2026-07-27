// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;

namespace Tawala.Projects
{
    /// <summary>
    /// A subclass of Field that represents a particular field in a record
    /// </summary>
    [Serializable]
    public class RecordField : Field, IAssignableField, IDeserializedField
    {
        private readonly Record record;
        private readonly IPaletteField referenceField;

        public RecordField(Record record, IPaletteField referenceField) : this(record, referenceField, true)
        {
        }

        public RecordField(Record record, IPaletteField referenceField, bool addToFieldMap)
        {
            this.referenceField = referenceField;
            this.record = record;

            if (addToFieldMap)
            {
                Project.FieldMapById.AddUnique(this);
            }
        }

        public Record Record { get { return record; } }

        public IField ReferenceField { get { return referenceField; } }

        #region IAssignableField Members

        /// <summary>
        /// Placed in documents, expressions, etc
        /// </summary>
        public override string FieldString
        {
            get
            {
                if (referenceField == null)
                {
                    return "<<" + record.FieldName + ":" + name + ">>";
                }
                else
                {
                    return "<<" + record.FieldName + ":" + referenceField.QualifiedFieldName + ">>";
                }
            }
        }

        public override string FieldName
        {
            get
            {
                if (referenceField == null)
                {
                    return record.FieldName + ":" + name;
                }
                else
                {
                    return record.FieldName + ":" + referenceField.FieldName;
                }
            }
        }

        public override string QualifiedFieldName
        {
            get
            {
                if (referenceField == null)
                {
                    return FieldName;
                }
                else
                {
                    return record.FieldName + ":" + referenceField.QualifiedFieldName;
                }
            }
        }

        // field's Record variable

        public override IEnumerable RecursiveEnumerator { get { yield return this; } }

        public string AssignmentName { get { return QualifiedFieldName; } }

        #endregion

        #region IOperatorDataSource

        public override IList OperatorDataSource { get { return ((IOperatorDataSource)referenceField).OperatorDataSource; } }

        #endregion

        #region IDeserializedField Members

        public IDeserializedField DeserializedFieldReference
        {
            get
            {
                var mappedField = (RecordField)Project.FieldMapById[Id];

                return (new RecordField(mappedField.Record, (IPaletteField)mappedField.ReferenceField));
            }
        }

        #endregion

        public override string ToString()
        {
            if (referenceField == null)
            {
                return record.FieldName + ":" + base.ToString();
            }
            else
            {
                if (FieldUtil.IsUnknownField(referenceField.QualifiedFieldName))
                {
                    return FieldUtil.UnknownFieldName;
                }
                return record.FieldName + ":" + referenceField.QualifiedFieldName;
            }
        }
    }
}