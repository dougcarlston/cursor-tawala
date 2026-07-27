// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Tawala.XmlSupport;
using Tawala.Common;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects.Forms.FormItemContents
{
	[Serializable]
	public class FieldReference : FormItemContents
	{
		string fieldName = string.Empty;
		IAnyField field;

		public FieldReference(IXmlElement element)
		{
			fieldName = string.Empty;

			if (element.HasAttribute("name"))
			{
				fieldName = element.GetAttribute("name");
			}
			else if (element.Text != null)
			{
				fieldName = element.Text.Replace("<<", "").Replace(">>", "");
			}

			ResolveFieldReferences();
		}

		public FieldReference(string fieldString)
		{
			fieldName = fieldString.Replace("<<", "").Replace(">>", "");

			ResolveFieldReferences();
		}

		[OnDeserialized]
		private void onDeserialized(StreamingContext context)
		{
			ResolveFieldReferences();
		}


		#region IFormItemContents Members

		public override string ToXml()
		{
			ResolveFieldReferences();
			return string.Format("<field name=\"{0}\"/>", field.QualifiedFieldName);
		}

		public override string ToXhtml(IFormItem formItem)
		{
			ResolveFieldReferences();
			return string.Format("<t:field fieldID=\"{0}\">{1}</t:field>", field.Id, field.QualifiedFieldName);
		}

		public override string Text
		{
			get
			{
				return "<<" + field.QualifiedFieldName + ">>";
			}
		}

		public override void ResolveFieldReferences()
		{
			if (field == null || field.QualifiedFieldName == "Unknown Field")
			{
				if (!string.IsNullOrEmpty(fieldName))
				{
					field = Project.FieldMapById.FindField(fieldName);
				}
			}
		}

		#endregion
	}
}
