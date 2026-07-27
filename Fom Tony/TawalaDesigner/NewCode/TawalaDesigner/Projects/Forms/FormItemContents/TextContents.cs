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
	public class TextContents : FormItemContents
	{
		protected string stringContents;

		protected TextContents()
		{
			
		}

		public TextContents(IXmlElement element)
		{
			stringContents = element.Value.Replace("~@!~", "");
		}

		#region IFormItemContents Members

		public override string ToXml()
		{
			return XMLStringFormatter.EscapeElementText(stringContents);
		}

		public override string ToXhtml(IFormItem formItem)
		{
			return XMLStringFormatter.EscapeElementText(stringContents);
		}

		public override string Text
		{
			get { return stringContents; }
		}

		#endregion
	}
}
