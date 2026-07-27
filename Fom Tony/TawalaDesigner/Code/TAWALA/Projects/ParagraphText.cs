// $Workfile: ParagraphText.cs $
// $Revision: 1 $	$Date: 4/06/06 7:24p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using Tawala.Common;
using Tawala.LabelSupport;
using Tawala.XmlSupport;

namespace Tawala.Proj
{
	[Serializable]
	public class ParagraphText : IParagraphComponent
	{
		public ParagraphText(string text)
		{
			this.text = text;
		}

		public ParagraphText(IXmlElement element)
		{
			this.text = element.OuterXml;
		}

		private string text = "";

		public string Text
		{
			get
			{
				return text;
			}
		}

		public string ToXml()
		{
			return text;
		}

		public string ToHtml()
		{
			return text;
		}
	}
}
