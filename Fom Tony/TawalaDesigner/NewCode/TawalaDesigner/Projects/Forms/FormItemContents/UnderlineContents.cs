// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
	[Serializable]
	public class UnderlineContents : FormItemContents
	{
		public UnderlineContents(IXmlElement element)
			: base(element)
		{
		}

		public override string ToXml()
		{
			return string.Format("<u>{0}</u>", Contents.ToXml());
		}

		public override string ToXhtml(IFormItem formItem)
		{
			return string.Format("<u>{0}</u>", Contents.ToXhtml(formItem));
		}
	}
}
