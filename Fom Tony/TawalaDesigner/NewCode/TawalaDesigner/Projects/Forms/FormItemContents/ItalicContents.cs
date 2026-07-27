// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
	[Serializable]
	public class ItalicContents : FormItemContents
	{
		public ItalicContents(IXmlElement element)
			: base(element)
		{
		}

		public override string ToXml()
		{
			return string.Format("<i>{0}</i>", Contents.ToXml());
		}

		public override string ToXhtml(IFormItem formItem)
		{
			return string.Format("<em>{0}</em>", Contents.ToXhtml(formItem));
		}
	}
}
