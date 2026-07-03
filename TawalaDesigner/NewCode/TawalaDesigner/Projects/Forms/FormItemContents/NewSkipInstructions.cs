// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
	[Serializable]
	public class NewSkipInstructions : Process, IFormItemContents, ISkipInstructions
	{
		public NewSkipInstructions()
			: this(new XmlElement("<skipInstructions></skipInstructions>"))
		{
		}

		public NewSkipInstructions(IXmlElement element)
		{
			foreach (IXmlElement childElement in element.GetChildren())
			{
				IProcessStatement statement = ProcessStatementFactory.MakeObject(childElement, this);
				Lines.Add(new ProcessLineList(statement as ProcessStatement));
			}
		}

		private static readonly string xmlSkipInstructionsStartTag = "<skipInstructions>\r\n";
		private static readonly string xmlSkipInstructionsEndTag = "</skipInstructions>\r\n";

		public override string ToXml()
		{
			return xmlSkipInstructionsStartTag + Lines.ToXml() + xmlSkipInstructionsEndTag;
		}

		public void ResolveFieldReferences()
		{
			foreach (ProcessLine processLine in Lines)
			{
				SkipToStatement skipToStatement = processLine.Statement as SkipToStatement;

				if (skipToStatement != null)
				{
					if (skipToStatement.Destination.ItemId == SkipToDestinationItem.InvalidItemId)
					{
						IForm form = getContainingForm();

						if (form != null)
						{
							string destinationName = form.Name + ":" + skipToStatement.Destination.FormItemLabel;

							foreach (IFormItem formItem in form.ItemList)
							{
								if (formItem.QualifiedFieldName == destinationName)
								{
									skipToStatement.Destination = new SkipToDestinationItem(formItem);
									break;
								}
							}
						}
					}
				}
			}
		}

		private IForm getContainingForm()
		{
			foreach (IForm form in Project.Current.FormList)
			{
				foreach (IFormItem formItem in form.ItemList)
				{
					ISkipInstructionsItem skipItem = formItem as ISkipInstructionsItem;

					if (skipItem != null)
					{
						FormItemContentsCollection skipInstructionsCollection = skipItem.Contents.GetDescendants(typeof(ISkipInstructions));

						foreach (IFormItemContents skipInstructions in skipInstructionsCollection)
						{
							if (skipInstructions == this)
							{
								return form;
							}
						}
					}
				}
			}

			return null;
		}

		#region IFormItemContents Members


		public string ToXhtml(IFormItem formItem)
		{
			throw new NotImplementedException();
		}

		public FormItemContentsCollection GetDescendants(Type descendantType)
		{
			FormItemContentsCollection descendants = new FormItemContentsCollection();

			if (descendantType.IsInstanceOfType(this))
			{
				descendants.Add(this);
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
			get { throw new NotImplementedException(); }
		}

		public void ResolveFunctionReferences()
		{
		}

		#endregion
	}
}
