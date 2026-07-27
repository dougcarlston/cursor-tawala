// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;
using Tawala.Common;

namespace Tawala.Projects.Forms.FormItemContents
{
	[Serializable]
	public class WhitespaceContents : FormItemContents
	{
		protected string stringContents;

		public WhitespaceContents(IXmlElement element)
		{
			if (element.Name.CompareTo("sp") == 0)
			{
				stringContents = " ";
			}
			else
			{
				stringContents = element.Value;
			}
		}

		#region IFormItemContents Members

		public override string ToXml()
		{
			return stringContents.Replace(" ", "<sp/>").Replace("\r", "").Replace("\n", "");
		}

		public override string ToXhtml(IFormItem formItem)
		{
			return " ";
		}

		#endregion
	}
}
