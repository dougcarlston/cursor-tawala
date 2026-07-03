// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text.RegularExpressions;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace Tawala.Projects.Fields
{
    public class FieldUtil
    {
        private static readonly Record record = new Record(DefaultRecordQualifierName);
        private static string formFieldPattern = @"^([^:]+?):([^:]+:[a-z]+|[^:]+)$";
        private static string recordFieldPattern = @"^([^:]+?):([^:]+?):([^:]+:[a-z]+|[^:]+)$";

        public static string UnknownFieldName { get { return "Unknown Field"; } }

        /// <summary>
        /// Default name for the record portion of a Record-qualified field.
        /// </summary>
        /// <remarks>
        /// NOTE: This name must agree with what the server expects for record fields in certain cases (e.g., Functions and Delete Statements);
        ///		  Therefore it is not helpful, and possible misleading, to keep it in a Resource file - jdf 3/07
        /// </remarks>
        public static string DefaultRecordQualifierName { get { return "Record"; } }

        public static Record DefaultRecordQualifierRecord { get { return record; } }

        public static bool IsVariable(string fieldString)
        {
            return !fieldString.Contains(":");
        }

        public static bool IsFormField(string fieldString)
        {
            string formName = Regex.Match(fieldString, formFieldPattern).Groups[1].Value;

            return Project.Current.GetForm(formName) != null;
        }

        public static bool IsExternalField(string fieldString)
        {
            string formName = Regex.Match(fieldString, formFieldPattern).Groups[1].Value;

            return FieldProviders.ExternalForms.IndexOf(formName) >= 0;
        }

        public static bool IsRecordField(string fieldString)
        {
            string formName = Regex.Match(fieldString, recordFieldPattern).Groups[2].Value;

            return Project.Current.GetForm(formName) != null;
        }

        public static bool IsExternalRecordField(string fieldString)
        {
            string formName = Regex.Match(fieldString, recordFieldPattern).Groups[2].Value;

            return FieldProviders.ExternalForms.IndexOf(formName) >= 0;
        }

        public static bool IsRegularOrExternalRecordField(string fieldString)
        {
            return IsRecordField(fieldString) || IsExternalRecordField(fieldString);
        }

        public static string GetRecordName(string fieldString)
        {
            return IsRecordField(fieldString) || IsExternalRecordField(fieldString)
                       ? Regex.Match(fieldString, recordFieldPattern).Groups[1].Value
                       : string.Empty;
        }

        public static string GetFormName(string fieldString)
        {
            if (IsFormField(fieldString) || IsExternalField(fieldString))
            {
                return Regex.Match(fieldString, formFieldPattern).Groups[1].Value;
            }
            else if (IsRecordField(fieldString) || IsExternalRecordField(fieldString))
            {
                return Regex.Match(fieldString, recordFieldPattern).Groups[2].Value;
            }

            return string.Empty;
        }

        public static string GetFieldName(string fieldString)
        {
            if (IsFormField(fieldString) || IsExternalField(fieldString))
            {
                return Regex.Match(fieldString, formFieldPattern).Groups[2].Value;
            }
            else if (IsRecordField(fieldString) || IsExternalRecordField(fieldString))
            {
                return Regex.Match(fieldString, recordFieldPattern).Groups[3].Value;
            }

            return fieldString;
        }

        public static bool IsUnknownField(string fieldString)
        {
            return (fieldString.Equals(UnknownFieldName));
        }

        public static IPaletteField RecordQualifyField(IPaletteField field)
        {
            return RecordQualifyField((IField)field) as IPaletteField;
        }

        public static IField RecordQualifyField(IField field)
        {
            var paletteField = field as IPaletteField;

            if (paletteField != null && !(paletteField is RecordField))
            {
                return new RecordField(DefaultRecordQualifierRecord, paletteField);
            }

            return field;
        }

        public static IField RecordQualifySharedDataField(IField field)
        {
            var paletteField = field as IPaletteField;

            if (paletteField is IBlank)
            {
                var b = paletteField as Blank;
                if (b != null && b.Owner != null && b.Owner.Form is ExternalForm)
                {
                    return new RecordField(DefaultRecordQualifierRecord, paletteField);
                }
            }

            if (paletteField is IMcqItem)
            {
                var mc = paletteField as IMcqItem;
                if (mc != null && mc.Form is ExternalForm)
                {
                    return new RecordField(DefaultRecordQualifierRecord, paletteField);
                }
            }

            return field;
        }

        private static IPaletteField resolveFormField(string fullyQualifiedFieldName)
        {
            string fieldName = GetFieldName(fullyQualifiedFieldName);
            string formName = GetFormName(fullyQualifiedFieldName);
            string recordName = GetRecordName(fullyQualifiedFieldName);

            if (string.IsNullOrEmpty(fieldName))
            {
                return null;
            }

            if (string.IsNullOrEmpty(formName))
            {
                return null;
            }

            if (!Project.Current.AllForms.ContainsComponentNamed(formName))
            {
                return null;
            }

            IForm form = Project.Current.AllForms[formName];

            var field = form.ItemList[fieldName] as IPaletteField;

            if (field == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(recordName))
            {
                return field;
            }
            else
            {
                return new RecordField(new Record(recordName), field);
            }
        }

        public static IPaletteField ResolveFormPaletteField(IPaletteField unresolvedField)
        {
            if (!(unresolvedField is UnresolvedPaletteField))
            {
                return unresolvedField;
            }

            IPaletteField resolvedField = resolveFormField(unresolvedField.QualifiedFieldName);

            return resolvedField != null ? resolvedField : unresolvedField;
        }

        public static IField ResolveFormField(IField unresolvedField)
        {
            if (!(unresolvedField is UnresolvedField))
            {
                return unresolvedField;
            }

            IPaletteField resolvedField = resolveFormField(unresolvedField.FieldName);

            return resolvedField != null ? resolvedField : unresolvedField;
        }

        /// <summary>
        /// Builds a list of all project Form Fields, Variables and pertinent Record Fields, as well as fields from Shared Data Sources
        /// </summary>
        public static FieldList GetGlobalFieldList(IXmlElement conditionsElement)
        {
            var allFields = new FieldList();

            foreach (IForm form in Project.Current.AllForms)
            {
                allFields.Add(form.GetAllFields());
            }

            allFields.Add(Project.Current.AllVariables);

            allFields.Add(getAllRecordFields(conditionsElement, new Record(DefaultRecordQualifierName, false)));

            return allFields;
        }

        private static IField getAllRecordFields(IXmlElement element, Record record)
        {
            var recordFields = new FieldList();

            foreach (IXmlElement childElement in element.GetChildren())
            {
                if (childElement.HasAttribute("field"))
                {
                    addRecordField(recordFields, record, childElement);
                }

                recordFields.Add(getAllRecordFields(childElement, record));
            }

            return recordFields;
        }

        private static void addRecordField(FieldList fieldList, Record record, IXmlElement childElement)
        {
            string fullFieldName = childElement.GetAttribute("field");

            if (IsRecordField(fullFieldName) || IsExternalRecordField(fullFieldName))
            {
                var recordField = new RecordField(record, GetFormField(fullFieldName));
                fieldList.AddUnique(recordField);
            }
        }

        public static IPaletteField GetFormField(string recordAndFormQualifiedName)
        {
            string formName = GetFormName(recordAndFormQualifiedName);
            string qualifiedFieldName = formName + ":" + GetFieldName(recordAndFormQualifiedName);

            var field = Project.Current.AllForms[formName].GetAllFields()[qualifiedFieldName] as IPaletteField;
            if (field == null)
            {
                field = PaletteField.NULL;
            }

            return field;
        }

        public static IPaletteField InitializeIdedField(string fieldName, int id)
        {
            if (IsUnknownField(fieldName))
            {
                return new Field(fieldName);
            }
            return (Project.FieldMapById.ContainsKey(id) ? (IPaletteField)Project.FieldMapById[id] : PaletteField.NULL);
        }

        public static IPaletteField InitializeNamedField(string fullFieldName)
        {
            IPaletteField referencedField = PaletteField.NULL;
            if (FieldUtil.IsUnknownField(fullFieldName))
            {
                referencedField = new Field(fullFieldName);
            }
            else if (FieldUtil.IsFormField(fullFieldName))
            {
                string formName = FieldUtil.GetFormName(fullFieldName);
                string fieldName = FieldUtil.GetFieldName(fullFieldName);

                if (Project.Current.GetForm(formName) != null)
                {
                    referencedField = (IPaletteField)Project.Current.GetForm(formName).ItemList[fieldName] ??
                                      new UnresolvedPaletteField(fullFieldName);
                }
            }
            else
            {
                referencedField = (Project.FieldMapByName.ContainsKey(fullFieldName)
                                       ? (IPaletteField)Project.FieldMapByName[fullFieldName]
                                       : PaletteField.NULL);
            }

            if (referencedField == PaletteField.NULL)
            {
                referencedField = new Variable(FieldUtil.GetFieldName(fullFieldName));
            }

            return referencedField;
        }
    }
}