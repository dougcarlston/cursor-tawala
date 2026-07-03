using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
	[Serializable]
	public class NewChoiceList : Collection<IChoice>, IChoiceList, IFormItemContents
	{
		#region IFormItemContents Members

		public NewChoiceList()
		{
		}

		public NewChoiceList(string[] choiceStrings)
		{
			int choiceNum = 0;

			foreach (string choiceString in choiceStrings)
			{
				string choiceLabel = new AlphaLabel(choiceNum++).ToString();
				Add(new NewChoice(choiceLabel, choiceString));
			}
		}

		public NewChoiceList(string choicesHtml)
		{
			int choiceNum = 0;

			string htmlString = Regex.Replace(choicesHtml, @"<\?xml:[^>]+>", "");

			foreach (IChoice choice in McqItemContentsFactory.MakeChildrenFromHtml(XmlUtility.ToXhtml(htmlString)) as FormItemContentsCollection)
			{
				choice.ChoiceLabel = new AlphaLabel(choiceNum++).ToString();
				Add(choice);
			}
		}

		public string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();

			foreach (IChoice choice in this)
			{
				xmlString.Append(choice.ToXml());
			}

			return xmlString.ToString();
		}

		public string ToXhtml(IFormItem formItem)
		{
			StringBuilder xhtmlString = new StringBuilder();

			foreach (IChoice choice in this)
			{
				xhtmlString.Append(choice.ToXhtml(formItem));
			}

			return xhtmlString.ToString();
		}

		public FormItemContentsCollection GetDescendants(Type descendantType)
		{
			FormItemContentsCollection descendants = new FormItemContentsCollection();

			if (descendantType.IsInstanceOfType(this))
			{
				descendants.Add(this);
			}

			foreach (IFormItemContents item in this)
			{
				foreach (IFormItemContents subItem in item.GetDescendants(descendantType))
				{
					descendants.Add(subItem);
				}
			}

			return descendants;
		}

		public IFormItemContents Contents
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void ApplyFontStyle(FontStyle style)
		{
			throw new NotImplementedException();
		}

		public FontStyle GetInnermostFontStyle()
		{
			throw new NotImplementedException();
		}

		public string Text
		{
			get
			{
				string text = string.Empty;

				for (int i = 0; i < this.Count; i++)
				{
					text += this[i].Text;

					if (!isLastChoiceInList(i))
					{
						text += Environment.NewLine;
					}
				}

				return text;
			}
		}

		private bool isLastChoiceInList(int choiceIndex)
		{
			return choiceIndex == (this.Count - 1);
		}

		public void ResolveFieldReferences()
		{
			foreach (IChoice choice in this)
			{
				choice.ResolveFieldReferences();
			}
		}

		public void ResolveFunctionReferences()
		{
			foreach (IChoice choice in this)
			{
				choice.ResolveFunctionReferences();
			}
		}

		#endregion

		#region IChoiceList Members

		public string GetLabel(int choiceIndex)
		{
			return new AlphaLabel(choiceIndex).ToString();
		}

		#endregion

		#region IField Members

		public string FieldName
		{
			get { throw new NotImplementedException(); }
		}

		public string FieldString
		{
			get { throw new NotImplementedException(); }
		}

		public IField this[string name]
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IRecursiveEnumerable Members

		public System.Collections.IEnumerable RecursiveEnumerator
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region IAnyField Members

		public int Id
		{
			get { throw new NotImplementedException(); }
		}

		public string QualifiedFieldName
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}
}
