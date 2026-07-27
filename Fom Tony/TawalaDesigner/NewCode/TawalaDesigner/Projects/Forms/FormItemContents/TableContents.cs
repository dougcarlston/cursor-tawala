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
	public abstract class TableContents : FormItemContents
	{
		protected TableContents(IXmlElement element)
		{
			Contents = FormItemContentsFactory.MakeChildren(element);
		}

		#region IFormItemContents Members

		public override string ToXml()
		{
			return string.Format("<table indent=\"0\">{0}</table>", Contents.ToXml());
		}

		public override string ToXhtml(IFormItem formItem)
		{
			return string.Format("<table class=\"user\" style=\"width: {0}pt;\"><tbody>{1}</tbody></table>", calculateTableWidth(), Contents.ToXhtml(formItem));
		}

        private int calculateTableWidth()
        {
            var rows = Contents.GetDescendants(typeof(TableRowContents));
            if (rows.Count == 0)
                return 0;

            var cells = rows[0].GetDescendants(typeof(TableCellContents));
            if (cells.Count == 0)
                return 0;

            int totalCellWidth = 0;

            foreach (TableCellContents cell in cells)
            {
                totalCellWidth += cell.WidthInTwips;
            }
            return totalCellWidth / 20;
        }

		#endregion
	}

	[Serializable]
	public class TableXmlContents : TableContents
	{
		public TableXmlContents(IXmlElement element)
			: base(element)
		{
		}
	}

	[Serializable]
	public class TableXhtmlContents : TableContents
	{
		public TableXhtmlContents(IXmlElement element)
			: base(element)
		{
		}
	}
}
