// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms.FormItemContents
{
	/// <summary>
	/// Converts any of the internal content elements (&lt;paragraph&gt;, &lt;table&gt;, &lt;b&gt;, etc.) of an XML form item to XHTML.
	/// Converts any of the internal content elements (&lt;p&gt;, &lt;table&gt;, &lt;strong&gt;, etc.) of an XHTML form item to XML.
	/// </summary>
	public class FormItemContentsConverter
	{
		private IFormItemContents contents;

		/// <summary>
		/// Constructs a converter from an XML or XHTML container element (e.g. &lt;container&gt;, &lt;text&gt;, etc.)
		/// </summary>
		public FormItemContentsConverter(IXmlElement containerElement)
		{
			contents = new ContentsContainer(containerElement);
		}

		public string ToXml()
		{
			return contents.ToXml();
		}

		public string ToXhtml(IFormItem formItem)
		{
			return contents.ToXhtml(formItem);
		}
	}
}
