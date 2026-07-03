// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Projects.Components;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Properties;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public class Document : Component, IDocument
    {
        private const string rawHtmlPostfix = "</body>\r\n" + "</html>";

        /// <summary>
        /// The HTML format for the TX text control (Version 12, SP 2)
        /// </summary>
        private const string rawHtmlPrefix =
            "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">\r\n" + "<html>\r\n" + "<head>\r\n" +
            "<meta content=\"TX_HTML32 12.0.230.500\" name=\"GENERATOR\">\r\n" + "<title></title>\r\n" + "</head>\r\n" +
            "<body bgcolor=\"#FFFFFF\" text=\"#000000\">\r\n";

        protected const string xmlDocumentEndTag = "</document>\r\n";
        protected const string xmlDocumentStartTag = "<document name=\"$DOCNAME\">\r\n";
        protected const string xmlHtmlDataEndTag = "]]>\r\n</htmlData>\r\n";
        protected const string xmlHtmlDataStartTag = "<htmlData>\r\n<![CDATA[";
        protected const string xmlRawHtmlDataEndTag = "]]>\r\n</rawHtmlData>\r\n";
        protected const string xmlRawHtmlDataStartTag = "<rawHtmlData>\r\n<![CDATA[";

        public static readonly IDocument NULL = new NullDocument("Null Document");
        protected Collection<IDocumentBlock> contents = new Collection<IDocumentBlock>();

        [NonSerialized]
        protected IXmlElement rawDataElement = XmlElement.NULL;

        /// <summary>
        /// Document text field.
        /// </summary>
        protected string text = "";

        public Document(string name) : base(name)
        {
        }

        // <summary>
        // Document text property.
        // </summary>

        public Collection<IDocumentBlock> Contents
        {
            get { return contents; }
            set { contents = value; }
        }

        public static string RawHtmlPrefix
        {
            get { return rawHtmlPrefix; }
        }

        public static string RawHtmlPostfix
        {
            get { return rawHtmlPostfix; }
        }

        #region IDocument Members

        public string Text
        {
            get { return text; }
            set { text = (value == null ? "" : value); }
        }

        public override string UserVisibleComponentTypeName
        {
            get { return Resources.DocumentComponentVisibleTypeName; }
        }

        /// <summary>
        /// Get list of fields in document
        /// </summary>
        public FieldList GetFields()
        {
            return getFieldList();
        }

        public override string ToXml()
        {
            var xmlString = new StringBuilder(xmlDocumentStartTag);

            xmlString.Replace("$DOCNAME", XMLStringFormatter.EscapeAttributeText(Name));

            // only HTML needed for the server
            xmlString.Append(xmlHtmlDataStartTag);

            string extractedBodyText = Regex.Match(text, @"<body[^>]+>\s*((.|\s)+)</body>").Groups[1].Value;

            xmlString.Append(extractedBodyText);
            xmlString.Append(xmlHtmlDataEndTag);

            // full HTML needed for text control when reloading from file
            xmlString.Append(xmlRawHtmlDataStartTag);
            xmlString.Append(text);
            xmlString.Append(xmlRawHtmlDataEndTag);

            xmlString.Append(xmlDocumentEndTag);

            return xmlString.ToString();
        }

        public IFormItemContents NewContents
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #endregion

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            rawDataElement = XmlElement.NULL;
        }

        protected virtual FieldList getFieldList()
        {
            var fieldList = new FieldList();

            string fieldString = (rawDataElement == XmlElement.NULL ? text : rawDataElement.InnerXml);

            var mc = Regex.Matches(fieldString, @"&lt;&lt;(\w+:[a-z]+)&gt;&gt;");

            for (int i = 0; i < mc.Count; i++)
            {
                // get display name (such as "Q1:a") from second matched group
                // (the one in parentheses in the regex pattern)
                // make field and add to local list
                fieldList.Add(new Field(mc[i].Groups[1].ToString()));
            }

            return fieldList;
        }

        #region Nested type: NullDocument

        [Serializable]
        private class NullDocument : Document
        {
            public NullDocument(string name) : base(name)
            {
            }
        }

        #endregion
    }
}