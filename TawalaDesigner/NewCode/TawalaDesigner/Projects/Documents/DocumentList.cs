// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using Tawala.XmlSupport;
using Tawala.Projects.Documents;

namespace Tawala.Projects
{
    [Serializable]
    public class DocumentList : ComponentList<IDocument>
    {
		static DocumentList()
		{
			xmlStartTag = "<documents>\r\n";
			xmlEndTag = "</documents>\r\n";
		}

		public DocumentList()
		{
		}

		public DocumentList(IXmlElement element)
		{
			foreach (IXmlElement childElement in element.GetChildren())
			{
				this.Add(ComponentMaker.MakeDocumentObject(childElement));
			}
		}
    }
}
