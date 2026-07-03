// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
	[Serializable]
	public class BoldContents : FormItemContents
	{
		public BoldContents(IXmlElement element)
			: base(element)
		{
		}

		public override string ToXml()
		{
			return string.Format("<b>{0}</b>", Contents.ToXml());
		}

		public override string ToXhtml(IFormItem formItem)
		{
			return string.Format("<strong>{0}</strong>", Contents.ToXhtml(formItem));
		}
	}
}
