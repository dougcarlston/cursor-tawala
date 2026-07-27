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
	public abstract class ParagraphContents : FormItemContents
	{
		protected int leftOffsetInTwips;
		private string alignment = "left";
		private bool contentsHaveTable;	

		protected ParagraphContents(IXmlElement element)
		{
			Contents = FormItemContentsFactory.MakeChildren(element);

			contentsHaveTable = element.HasChild("table");

			if (element.HasAttribute("align"))
			{
				alignment = element.GetAttribute("align");
			}
		}

		#region IFormItemContents Members

		public override string ToXml()
		{
		    if (contentsHaveTable)
			{
				return Contents.ToXml();
			}
		    return string.Format("<paragraph indent=\"{0}\" align=\"{1}\">{2}</paragraph>", leftOffsetInTwips, alignment, Contents.ToXml());
		}

	    public override string ToXhtml(IFormItem formItem)
		{
			return string.Format("<p style=\"margin-left: {0}pt\" align=\"{1}\">{2}</p>", leftOffsetInTwips / 20, alignment, Contents.ToXhtml(formItem));
		}

		#endregion
	}

	[Serializable]
	public class ParagraphXmlContents : ParagraphContents
	{
		public ParagraphXmlContents(IXmlElement element)
			: base(element)
		{
			leftOffsetInTwips = Convert.ToInt32(element.GetAttribute("indent"));
		}
	}

	[Serializable]
	public class ParagraphXhtmlContents : ParagraphContents
	{
		public ParagraphXhtmlContents(IXmlElement element)
			: base(element)
		{
			if (element.HasAttribute("style"))
			{
				string style = element.GetAttribute("style");

				Match match = Regex.Match(style, "MARGIN-LEFT: (\\d+)pt", RegexOptions.IgnoreCase);
				leftOffsetInTwips = Convert.ToInt32(match.Groups[1].Value) * 20;
			}
		}
	}
}
