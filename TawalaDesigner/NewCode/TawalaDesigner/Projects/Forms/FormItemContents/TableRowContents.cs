// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Projects;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
	[Serializable]
	public abstract class TableRowContents : FormItemContents
	{
		protected TableRowContents(IXmlElement element)
		{
			Contents = FormItemContentsFactory.MakeChildren(element);
		}

		#region IFormItemContents Members

		public override string ToXml()
		{
			return string.Format("<row>{0}</row>", Contents.ToXml());
		}

		public override string ToXhtml(IFormItem formItem)
		{
			return string.Format("<tr style=\"height: 12pt\">{0}</tr>", Contents.ToXhtml(formItem));
		}

		#endregion
	}

	[Serializable]
	public class TableRowXmlContents : TableRowContents
	{
		public TableRowXmlContents(IXmlElement element)
			: base(element)
		{
		}
	}

	[Serializable]
	public class TableRowXhtmlContents : TableRowContents
	{
		public TableRowXhtmlContents(IXmlElement element)
			: base(element)
		{
		}
	}
}
