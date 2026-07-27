// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text.RegularExpressions;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
    [Serializable]
    public abstract class TableCellContents : FormItemContents
    {
        private readonly bool contentsIsDivision;

        protected TableCellContents(IXmlElement element)
        {
            FormItemContentsCollection collection = FormItemContentsFactory.MakeChildren(element);
            contentsIsDivision = (collection.Count == 1 && collection[0] is DivisionContents);
            Contents = collection;
        }

        public int WidthInTwips { get; protected set; }

        public override string ToXml()
        {
            if (contentsIsDivision)
            {
                return string.Format("<cell width=\"{0}\">{1}</cell>", WidthInTwips, Contents.ToXml());
            }
            return string.Format("<cell width=\"{0}\"><division indent=\"0\" align=\"left\"><font>{1}</font></division></cell>",
                                 WidthInTwips, Contents.ToXml());
        }

        public override string ToXhtml(IFormItem formItem)
        {
            return string.Format("<td style=\"width: {0}pt\">{1}</td>", (double)WidthInTwips/20, Contents.ToXhtml(formItem));
        }
    }

    [Serializable]
    public class TableCellXmlContents : TableCellContents
    {
        public TableCellXmlContents(IXmlElement element)
            : base(element)
        {
            WidthInTwips = Convert.ToInt32(element.GetAttribute("width"));
        }
    }

    [Serializable]
    public class TableCellXhtmlContents : TableCellContents
    {
        public TableCellXhtmlContents(IXmlElement element)
            : base(element)
        {
            string style = element.GetAttribute("style");
            Match match = Regex.Match(style, @"WIDTH: (\d+\.?\d*)pt");
            WidthInTwips = Convert.ToInt32(Convert.ToDouble(match.Groups[1].Value)*20);
        }
    }
}