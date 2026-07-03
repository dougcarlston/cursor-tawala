// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    [Serializable]
    public class HiddenField : FormItem, IAssignableField, IDeserializedField, IHiddenField
    {
        private string name = string.Empty;

        public HiddenField()
        {
            name = createDefaultName();
            Project.FieldMapById.AddUnique(this);
            Project.FieldMapByName.AddUnique(this);
        }

        public HiddenField(IXmlElement element)
        {
            name = element.GetAttribute("name");
            Project.FieldMapById.AddUnique(this);
            Project.FieldMapByName.AddUnique(this);
        }

        #region IAssignableField Members

        public string AssignmentName { get { return QualifiedFieldName; } }

        #endregion

        #region IDeserializedField Members

        public IDeserializedField DeserializedFieldReference { get { return (IDeserializedField)Project.FieldMapById[Id]; } }

        #endregion

        #region IHiddenField Members

        public string Name
        {
            get { return name; }
            set
            {
                Project.FieldMapByName.Remove(this);

                name = value;

                Project.FieldMapByName.AddUnique(this);
            }
        }

        public override string ToXml()
        {
            return string.Format("<field name=\"{0}\"/>\r\n", XMLStringFormatter.EscapeAttributeText(name));
        }

        #endregion

        #region IPaletteField Interface

        public override string FieldName { get { return Form == null ? FieldUtil.UnknownFieldName : name; } }

        public override string QualifiedFieldName
        {
            get
            {
                if (!Project.Current.AllForms.ContainsComponent(Form))
                {
                    return FieldUtil.UnknownFieldName;
                }
                else
                {
                    return Form.Name + ":" + FieldName;
                }
            }
        }

        public override IField this[string nameParam]
        {
            get
            {
                if (FieldName == nameParam)
                {
                    return this;
                }

                return null;
            }
        }

        #endregion

        #region IEnumerable Interface

        public override IEnumerator GetEnumerator()
        {
            yield return this;
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public override IEnumerable RecursiveEnumerator { get { yield return this; } }

        #endregion

        #region IOperatorDataSource

        public IList OperatorDataSource { get { return HybridOperator.List.DataSource; } }

        #endregion

        public bool IsNameUniqueInForm(string name)
        {
            if (form == null || form == Projects.Form.NULL)
            {
                return false;
            }

            FieldList fieldList = form.GetFormItemFields();

            foreach (IField field in fieldList)
            {
                if (field.FieldName.CompareTo(name) == 0)
                {
                    return ReferenceEquals(field, this);
                }
            }

            return false;
        }

        private static string createDefaultName()
        {
            var regex = new Regex("Field([0-9]+)");

            int num = 1;

            foreach (IForm form in Project.Current.FormList)
            {
                FieldList fieldList = form.GetAllFields();

                foreach (IField field in fieldList.RecursiveEnumerator)
                {
                    Match match = regex.Match(field.FieldName);

                    if (match.Success)
                    {
                        Group group = match.Groups[1];

                        int value = Convert.ToInt32(group.Value);
                        if (value >= num)
                        {
                            num = value + 1;
                        }
                    }
                }
            }
            return string.Format("Field{0}", num);
        }

        public override string ToString()
        {
            return QualifiedFieldName;
        }
    }
}