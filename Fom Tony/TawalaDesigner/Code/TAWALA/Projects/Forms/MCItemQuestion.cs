// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms
{
    /// <summary>
    /// Class to contain the question portion (first line) of a multiple choice question
    /// </summary>
    [Serializable]
    public class MCItemQuestion : IDocumentBlock
    {
        private static readonly ChoiceBlockFactory<IDocumentBlock> blockFactory = new ChoiceBlockFactory<IDocumentBlock>();
        private static readonly string xmlMCQuestionEndTag = "</question>";
        private static readonly string xmlMCQuestionStartTag = "<question>";

        private readonly Collection<IDocumentBlock> contents = new Collection<IDocumentBlock>();

        static MCItemQuestion()
        {
            blockFactory.Register("paragraph", typeof(FormItemParagraph));
        }

        public MCItemQuestion(IXmlElement element)
        {
            if (element.HasChild("paragraph"))
            {
                contents = makeContentsFromParagraph(element);
            }
            else
            {
                contents = makeContentsFromText(element.Text);
            }
        }

        public MCItemQuestion(string text)
        {
            contents = makeContentsFromText(text);
        }

        public Collection<IDocumentBlock> Contents { get { return contents; } }

        #region IDocumentBlock Members

        public string Text
        {
            get
            {
                var text = new StringBuilder();

                foreach (IDocumentBlock block in contents)
                {
                    text.Append(block.Text);
                }

                return text.ToString();
            }
        }

        public string ToXml()
        {
            var xmlString = new StringBuilder();

            xmlString.Append(xmlMCQuestionStartTag);

            foreach (IDocumentBlock block in contents)
            {
                xmlString.Append(block.ToXml());
            }

            xmlString.Append(xmlMCQuestionEndTag);

            return xmlString.ToString();
        }

        public string ToHtml()
        {
            return String.Empty;
        }

        public string ToRtf()
        {
            var rtfString = new StringBuilder();

            foreach (IDocumentBlock block in contents)
            {
                rtfString.Append(block.ToRtf());
            }

            return rtfString.ToString();
        }

        public string ToRtf(RtfDocument document)
        {
            var rtfString = new StringBuilder();

            foreach (IDocumentBlock block in contents)
            {
                if (block != null)
                {
                    rtfString.Append(block.ToRtf(document));
                }
            }

            return rtfString.ToString();
        }

        #endregion

        /// <summary>
        /// Builds the question's contents from the XML string that starts with a &lt;question&gt; element
        /// and contains one or more <paragraph> elements.
        /// </summary>
        private Collection<IDocumentBlock> makeContentsFromParagraph(IXmlElement element)
        {
            var contents = new Collection<IDocumentBlock>();

            Collection<XmlElement> blockElements = element.GetChildren();

            foreach (IXmlElement blockElement in blockElements)
            {
                contents.Add(blockFactory.MakeObject(blockElement));
            }

            return contents;
        }

        /// <summary>
        /// Builds the choice's contents from the specified text.
        /// </summary>
        private Collection<IDocumentBlock> makeContentsFromText(string text)
        {
            string xmlParagraphFormat = "<question>" + "<paragraph indent=\"0\" align=\"left\">" +
                                        "<font face=\"Arial\" size=\"200\" color=\"000000\">" + "{0}" + "</font>" + "</paragraph>" +
                                        "</question>";

            string xmlString = String.Format(xmlParagraphFormat, text);
            IXmlElement element = new XmlElement(xmlString);

            return makeContentsFromParagraph(element);
        }
    }
}