// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects.Forms.FormItemContents
{
	public class ContentsContainer : FormItemContents
	{
		public ContentsContainer(IXmlElement element)
		{
			Contents = FormItemContentsFactory.MakeChildren(element);
		}

		#region IFormItemContents Members

		public override string ToXml()
		{
			return string.Format("<container>{0}</container>", Contents.ToXml());
		}

		public override string ToXhtml(IFormItem formItem)
		{
			return string.Format("{0}", Contents.ToXhtml(formItem));
		}

		#endregion
	}
}
