// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Functions.Runtime;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    /// <summary>
    /// Class intended primarily to produce XML for Functions representing multiple forms.
    /// </summary>
    public class FunctionFormCollection : Collection<IForm>, IFunctionParameterValue
    {
        private const string xmlExternalFormTag = "<form name=\"{0}\" externalSharedData=\"true\" />";
        private const string xmlFormTag = "<form name=\"{0}\" />";
        public static FunctionFormCollection NULL = new FunctionFormCollection();

        public FunctionFormCollection()
        {
        }

        public FunctionFormCollection(IForm form)
        {
            if (!Contains(form))
            {
                if (form != null)
                {
                    Add(form);
                }
            }
        }

        public FunctionFormCollection(Collection<XmlElement> formElements)
        {
            foreach (IXmlElement formElement in formElements)
            {
                IForm form = Project.Current.AllForms[formElement.GetAttribute("name")];

                AddUnique(form ?? Form.NULL);
            }
        }

        #region IFunctionParameterValue Members

        public string ToValueXml()
        {
            return ToXml();
        }

        public string FormattedString { get { throw new NotImplementedException(); } }

        #endregion

        public void AddUnique(IForm form)
        {
            if (!Contains(form))
            {
                if (form != null)
                {
                    Add(form);
                }
            }
        }

        public override bool Equals(object obj)
        {
            var other = (FunctionFormCollection)obj;

            if (Count != other.Count)
            {
                return false;
            }

            foreach (IForm form in this)
            {
                if (!other.Contains(form))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public FieldList GetAllFields()
        {
            var fields = new FieldList();

            foreach (IForm form in this)
            {
                if (form != null)
                {
                    foreach (IField field in form.GetAllFields())
                    {
                        fields.AddUnique(field);
                    }
                }
            }

            return fields;
        }

        public string ToXml()
        {
            var xmlString = new StringBuilder();

            foreach (IForm f in this)
            {
                IForm form = f;

                if (form == null || !Project.Current.AllForms.Contains(form))
                {
                    form = Form.NULL;
                }

                string format = form is ExternalForm ? xmlExternalFormTag : xmlFormTag;
                xmlString.Append(string.Format(format, form.Name));
            }

            return xmlString.ToString();
        }
    }
}