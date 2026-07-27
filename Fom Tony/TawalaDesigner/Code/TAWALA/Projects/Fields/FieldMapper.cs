// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tawala.Projects.Fields
{
    /// <summary>
    /// Class to map text strings (such as "Q1:a" and "Record:Score") to the actual fields they represent.
    /// This class is used in converting XML elements to their respective Tawala objects.
    /// </summary>
    public class FieldMapper : Dictionary<string, IField>
    {
        /// <summary>
        /// List of all fields, qualified and unqualified.
        /// </summary>
        private readonly FieldList allFields = new FieldList();

        /// <summary>
        /// List of fields, such as "Q1:a".
        /// </summary>
        private readonly FieldList fields = new FieldList();

        /// <summary>
        /// List of field qualifiers, such as "Record 1"
        /// </summary>
        private readonly Collection<string> qualifiers = new Collection<string>();

        private string parentName = "";

        public FieldMapper()
        {
        }

        public FieldMapper(string parentName)
        {
            this.parentName = parentName;
        }

        public FieldList AllFields { get { return allFields; } }

        public FieldList Fields { get { return fields; } }

        public Collection<string> Qualifiers { get { return qualifiers; } }

        /// <summary>
        /// Places all combinations of qualifiers and fields into the field mapper
        /// </summary>
        public void Map()
        {
            Clear();
            allFields.Clear();

            mapUnqualifiedFields();

            mapFormQualifiedFields();

            mapRecordQualifiedFields();
        }

        /// <summary>
        /// Places unqualified fields (e.g. "Q1:a") into the field mapper
        /// </summary>
        private void mapUnqualifiedFields()
        {
            foreach (IField field in fields.RecursiveEnumerator)
            {
                string key = field.FieldName;

                if (!ContainsKey(key))
                {
                    this[key] = field;
                    allFields.Add(field);
                }
            }
        }

        /// <summary>
        /// Places form-qualified fields (e.g. "Form1:Q1:a") into the field mapper
        /// </summary>
        private void mapFormQualifiedFields()
        {
            foreach (IField field in fields.RecursiveEnumerator)
            {
                string key = field.ToString();

                if (!ContainsKey(key))
                {
                    this[key] = field;
                    allFields.Add(field);
                }
            }
        }

        /// <summary>
        /// Places record-qualified fields (e.g. "Record1:Q1:a", "Record1:Form 1:Q1:a") into the field mapper
        /// </summary>
        private void mapRecordQualifiedFields()
        {
            foreach (string qualifier in qualifiers)
            {
                foreach (IPaletteField field in fields.RecursiveEnumerator)
                {
                    string recordQualifiedKey = qualifier + ":" + field.FieldName;
                    string recordAndFormQualifiedKey = qualifier + ":" + field.QualifiedFieldName;

                    if (!ContainsKey(recordQualifiedKey))
                    {
                        var qualifiedField = new RecordField(new Record(qualifier, false), field, false);
                        this[recordQualifiedKey] = qualifiedField;
                        allFields.Add(qualifiedField);
                    }

                    if (!ContainsKey(recordAndFormQualifiedKey))
                    {
                        var qualifiedField = new RecordField(new Record(qualifier, false), field, false);
                        this[recordAndFormQualifiedKey] = qualifiedField;
                        allFields.Add(qualifiedField);
                    }
                }
            }
        }

        public void Empty()
        {
            Clear();
            fields.Clear();
            allFields.Clear();
            qualifiers.Clear();
        }
    }
}