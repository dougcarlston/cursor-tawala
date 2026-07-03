// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects;
using Tawala.XmlSupport;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects.Forms.FormItemContents
{
	public class BlankPlaceholderXhtmlContents : FormItemContents
	{
		private string label = "b";

		public BlankPlaceholderXhtmlContents(IXmlElement element)
		{
		}

		#region IFormItemContents Members

		public override string ToXml()
		{
			return string.Empty;
		}

		public override string ToXhtml(IFormItem formItem)
		{
			string format =
				@"<t:Blank id=""1002"">" +
				@"<input class=""blank"" size=""20"" style=""height:21px;"" value=""{0}"" />" +
				@"</t:Blank>";

			return String.Format(format, label);
		}

		#endregion
	}
}
