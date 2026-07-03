// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using Tawala.Common;
using Tawala.XmlSupport;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects.Forms.FormItemContents
{
	[Serializable]
	public abstract class NewBlank : FormItemContents, IBlank, IDeserializedField, IAssignableField
	{
		protected string label;
		protected string alternateLabel = "";
		protected int length;
		protected int height;
		protected bool required;
		protected int id;

		protected NewBlank(IXmlElement element) : base(element)
		{
		}

        protected NewBlank(IFibItem fibItem)
        {
            owner = fibItem;
        }

		[OnDeserialized]
		internal void onDeserialized(StreamingContext context)
		{
			id = Project.NextUniqueID;
			Project.FieldMapById.AddUnique(this);
		}

		private IFibItem owner;

		private IFibItem blankOwner
		{
			get
			{
				if (owner == null)
				{
					owner = getOwnerFromFormList();
				}

				return owner;
			}
		}

		private IFibItem getOwnerFromFormList()
		{
			foreach (IForm form in Project.Current.AllForms)
			{
				foreach (FormItem formItem in form.ItemList)
				{
					IFibItem fibItem = formItem as NewFibItem;

					if (fibItem != null)
					{
						if (fibItem.BlankList.Contains(this))
						{
							return fibItem;
						}
					}
				}
			}

			return null;
		}

		#region IFormItemContents Members

		public override string ToXml()
		{
			if (alternateLabel != "")
			{
				string format = "<blank label=\"{0}\" length=\"{1}\" height=\"{2}\" required=\"{3}\" alternateLabel=\"{4}\"/>";
				return string.Format(format, getAlphaLabel(), length, height, required.ToString().ToLower(), alternateLabel);
			}
			else
			{
				string format = "<blank label=\"{0}\" length=\"{1}\" height=\"{2}\" required=\"{3}\"/>";
				return string.Format(format, getAlphaLabel(), length, height, required.ToString().ToLower());
			}
		}

		public override string ToXhtml(IFormItem formItem)
		{
			string format = "<t:Blank id=\"{0}\"><input class=\"blank\" size=\"{1}\" style=\"height:{2}px;\" value=\"{3}\" /></t:Blank>";
			return String.Format(format, id, length, height * 21, getFieldName());
		}

		#endregion

		#region IBlank Interface

		public bool Required
		{
			get { return required; }
			set { required = value; }
		}

		public int Length
		{
			get { return length; }
			set { length = value; }
		}

		#endregion

		#region IFormField Interface

		public string FieldName
		{
			get { return (AlternateLabel != "" ? AlternateLabel : getFieldName()); }
		}

		private string getFieldName()
		{
			return (blankOwner == null ? "BlankWithNoOwner" : blankOwner.FieldName + ":" + getAlphaLabel());
		}

		private string getAlphaLabel()
		{
			return (blankOwner == null ? label : new AlphaLabel(blankOwner.BlankList.IndexOf(this)).ToString());
		}

		public string QualifiedFieldName
		{
			get
			{
				IForm form;

                if ((form = getFormContaining()) == NullObjects.Form)
				{
					return FieldUtil.UnknownFieldName;
				}

				return form.Name + ":" + FieldName;
			}
		}

		private IForm getFormContaining()
		{
			if (blankOwner != null && Project.Current.AllForms.ContainsComponent(blankOwner.Form))
			{
				return blankOwner.Form;
			}

            return NullObjects.Form;
		}

		public string AlternateLabel
		{
			get
			{
				return alternateLabel;
			}
			set
			{
				Project.FieldMapByName.Remove(this);
				alternateLabel = value.Trim();
				Project.FieldMapByName.AddUnique(this);
				Project.Events.RaiseFormItemChangedEvent(new FormItemEventArgs(null, blankOwner as FormItem, -1));
			}
		}

		public int Id
		{
			get { return id; }
		}

		public override string ToString()
		{
			return QualifiedFieldName;
		}

		#endregion

		#region IField Members


		public string FieldString
		{
			get { return "<<" + QualifiedFieldName + ">>"; }
		}

		public IField this[string name]
		{
			get
			{
				if (name == FieldName)
				{
					return this;
				}

				return null;
			}
		}

		#endregion

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			yield return this;
		}

		#endregion

		#region IRecursiveEnumerable Members

		public System.Collections.IEnumerable RecursiveEnumerator
		{
			get
			{
				yield return this;
			}
		}

		#endregion

		#region IDeserializedField Members

		public IDeserializedField DeserializedFieldReference
		{
			get
			{
				return (IDeserializedField)Project.FieldMapById[Id];
			}
		}

		#endregion

		#region IOperatorDataSource Members

		public System.Collections.IList OperatorDataSource
		{
			get
			{
				return HybridOperator.List.DataSource;
			}
		}

		#endregion

		#region IAssignableField Members

		public string AssignmentName
		{
			get { return QualifiedFieldName; }
		}

		#endregion
	}

    [Serializable]
    public class ExternalBlank : NewBlank
    {
        public ExternalBlank(IFibItem fibItem, string altLabel) : base(fibItem)
        {
            id = Project.NextUniqueID;
            alternateLabel = altLabel;
            length = 1;
            height = 1;
        }
    }

	[Serializable]
	public class XmlBlank : NewBlank
	{
		public XmlBlank(IXmlElement element)
			: base(element)
		{
			id = Project.NextUniqueID;
			label = element.GetAttribute("label");
			length = Convert.ToInt32(element.GetAttribute("length"));
			height = getHeight(element);
			required = Convert.ToBoolean(element.GetAttribute("required"));

			alternateLabel = getAlternateLabel(element);
			
			Project.FieldMapById.AddUnique(this);
		}

		private string getAlternateLabel(IXmlElement element)
		{
			if (element.HasAttribute("alternateLabel"))
			{
				return element.GetAttribute("alternateLabel");
			}

			return string.Empty;
		}

		private int getHeight(IXmlElement element)
		{
			int blankHeight = 1;

			if (element.HasAttribute("height"))
			{
				blankHeight = Convert.ToInt32(element.GetAttribute("height"));
			}

			return blankHeight;
		}
	}

	[Serializable]
	public class XhtmlBlank : NewBlank
	{
		public XhtmlBlank(IXmlElement element) : this(element, true)
		{
		}

		protected XhtmlBlank(IXmlElement element, bool addToFieldMap)
			: base(element)
		{
			id = Convert.ToInt32(element.GetAttribute("id"));
			length = getLength(element);
			height = getHeight(element);
			alternateLabel = getAlternateLabel(element);
			required = getRequired(id);

			if (addToFieldMap)
			{
				Project.FieldMapById.AddUnique(this);
			}
		}

		public static NewBlank MakeHtmlBlank(IXmlElement element)
		{
			NewBlank freshBlank = new XhtmlBlank(element, false);

			int id = Convert.ToInt32(element.GetAttribute("id"));

			if (Project.FieldMapById.ContainsKey(id))
			{
				NewBlank cachedBlank = Project.FieldMapById[id] as NewBlank;

				if (cachedBlank != null)
				{
					if (cachedBlank.Length == freshBlank.Length)
					{
						return cachedBlank;
					}
				}
			}

			Project.FieldMapById.AddUnique(freshBlank);

			return freshBlank;
		}

		private const int linePixelHeight = 21;

		private int getHeight(IXmlElement element)
		{
			IXmlElement input = element.GetChild("input");
			string style = input.GetAttribute("style");

			Match match = Regex.Match(style, "HEIGHT:[ ]*(\\d+)px", RegexOptions.IgnoreCase);
			return match != null ? Convert.ToInt32(match.Groups[1].Value) / linePixelHeight : 1;
		}

		private int getLength(IXmlElement element)
		{
			IXmlElement input = element.GetChild("input");
			return (input.HasAttribute("size") ? Convert.ToInt32(input.GetAttribute("size")) : 20);
		}

		private string getAlternateLabel(IXmlElement element)
		{
			IXmlElement inputElement = element.GetChild("input");

			if (inputElement.HasAttribute("value"))
			{
				string valueString = inputElement.GetAttribute("value");

				if (representsAlternateLabel(valueString))
				{
					alternateLabel = valueString;
				}
			}

			return alternateLabel;
		}

		private static bool representsAlternateLabel(string valueString)
		{
			return !valueString.Contains(":");
		}

		private bool getRequired(int blankId)
		{
			if (Project.FieldMapById.ContainsKey(blankId))
			{
				IBlank blank = Project.FieldMapById[blankId] as IBlank;

				if (blank != null)
				{
					return blank.Required;
				}
			}

			return required;
		}
	}
}
