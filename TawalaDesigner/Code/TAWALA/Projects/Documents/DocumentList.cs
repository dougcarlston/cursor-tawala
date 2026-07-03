// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Components;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
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
            foreach (var childElement in element.GetChildren())
            {
                Add(ComponentMaker.MakeDocumentObject(childElement));
            }
        }
    }
}