// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Tawala.Functions.Runtime;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	/// <summary>
	/// Class intended primarily to produce XML for Functions representing multiple forms.
	/// </summary>
	public class FunctionFormCollection : Collection<IForm>, IFunctionParameterXml
	{
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
				
				AddUnique(form == null? NullObjects.Form : form);
			}
		}

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
			FunctionFormCollection other = (FunctionFormCollection)obj;

			if (this.Count != other.Count)
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
			FieldList fields = new FieldList();

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

		private const string xmlExternalFormTag = "<form name=\"{0}\" externalSharedData=\"true\" />";
		private const string xmlFormTag = "<form name=\"{0}\" />";

		public string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();

			foreach (IForm f in this)
			{
				IForm form = f;

				if (form == null || !Project.Current.AllForms.Contains(form))
				{
                    form = NullObjects.Form;
				}

				string format = form is IExternalForm ? xmlExternalFormTag : xmlFormTag;
				xmlString.Append(string.Format(format, form.Name));
			}

			return xmlString.ToString();
		}

        #region IFunctionParameterXml Members

        public string ToFunctionParameterXml()
        {
            return ToXml();
        }

        #endregion
    }
}
