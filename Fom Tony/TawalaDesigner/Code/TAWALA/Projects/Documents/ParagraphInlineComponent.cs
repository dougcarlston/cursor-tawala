// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public class ParagraphInlineComponent : ParagraphComponent
    {
        protected IParagraphComponent contents = NULL;

        public ParagraphInlineComponent()
        {
        }

        public ParagraphInlineComponent(IXmlElement element)
        {
            var contentsList = new ParagraphComponentList();

            foreach (XmlElement childElement in element.GetChildren())
            {
                IParagraphComponent component = Paragraph.ComponentFactory.MakeObject(childElement);
                contentsList.Add((component ?? NULL));
            }

            contents = contentsList;
        }

        public IParagraphComponent Contents { get { return contents; } }

        public override string Text { get { return contents.Text; } }
    }
}