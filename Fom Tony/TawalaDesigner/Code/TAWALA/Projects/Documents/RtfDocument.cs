// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.FontSupport;
using Tawala.Projects.Factories;
using Tawala.Projects.Fields;
using Tawala.RtfSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public class RtfDocument : Document
    {
        private const string NEWLINE = "\r\n";
        private const string rtfDefaultTabs = @"\deftab0\tx2880";
        private const string rtfEnd = @"}";
        private const string rtfProlog = @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + NEWLINE;
        private const string rtfStringPostfix = @"}";

        private const string rtfStringPrefix =
            @"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + @"{\f0\fswiss\fcharset0\fprq2 Arial;}" +
            @"{\f1\froman\fcharset2\fprq2 Symbol;}}" + @"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" +
            @"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" +
            @"{\*\generator TX_RTF32 12.0.500.502;}" + @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind" +
            @"{\pard\itap0\sb2\sa2\plain\f0\fs20 ";

        protected const string xmlDataEndTag = "\r\n</xmlData>\r\n";
        protected const string xmlDataStartTag = "<xmlData>\r\n";
        protected const string xmlRtfDataEndTag = "]]>\r\n</rtfData>\r\n";
        protected const string xmlRtfDataStartTag = "<rtfData>\r\n<![CDATA[";
        private static readonly Factory<IDocumentBlock> blockFactory = new Factory<IDocumentBlock>();

        public new static RtfDocument NULL = new NullRtfDocument("Null RTF Document");
        private readonly RtfColorTable colorTable = new RtfColorTable();
        private readonly RtfFontTable fontTable = new RtfFontTable();

        private string parserXmlString = "";
        private string rtfString = "";

        static RtfDocument()
        {
            blockFactory.Register("paragraph", typeof(Paragraph));
            blockFactory.Register("table", typeof(Table));
        }

        public RtfDocument(string name) : base(name)
        {
        }

        public RtfDocument(IXmlElement element) : this(element.GetAttribute("name"))
        {
            IXmlElement xmlData = element.GetChild("xmlData");

            buildColorTable(xmlData);
            buildFontTable(xmlData);

            if (xmlData != XmlElement.NULL)
            {
                contents = makeContents(xmlData.OuterXml);
            }
            else
            {
                rawDataElement = element.GetChild("rtfData");

                if (rawDataElement != XmlElement.NULL)
                {
                    Rtf = rawDataElement.GetChild("#cdata-section").Value ?? "";
                }
                else
                {
                    // old text & field, or HTML format
                    constructOldTextProperty(element);
                }
            }
        }

        public static string RtfStringPrefix { get { return rtfStringPrefix; } }

        public static string RtfStringPostfix { get { return rtfStringPostfix; } }

        public string Rtf
        {
            get { return ToRtf(); }
            set
            {
                rtfString = value;

                if (rtfString != null)
                {
                    var parser = new RtfParser(rtfString, fontTable, colorTable);
                    parser.Parse();

                    parserXmlString = parser.ToXml();

                    contents = makeContents(xmlDataStartTag + parserXmlString + xmlDataEndTag);
                }
            }
        }

        public RtfFontTable FontTable { get { return fontTable; } }

        public RtfColorTable ColorTable { get { return colorTable; } }

        /// <summary>
        /// Builds a font table from the "<font>" elements that are descendants of the specified element. 
        /// </summary>
        private void buildFontTable(IXmlElement element)
        {
            Collection<XmlElement> fontElements = element.GetDescendants("font");

            fontTable.AddUnique(new RtfFontTableEntry("swiss", "Arial"));
            //			fontTable.AddUnique(new RtfFontTableEntry("roman", "Symbol"));
            fontTable.AddUnique(new RtfFontTableEntry("nil", Fonts.DefaultFontName));

            foreach (XmlElement fontElement in fontElements)
            {
                fontTable.AddUnique(new RtfFontTableEntry(fontElement));
            }
        }

        /// <summary>
        /// Builds a color table from the "<font>" elements that are descendants of the specified element. 
        /// </summary>
        private void buildColorTable(IXmlElement element)
        {
            // always place black and white into table
            colorTable.Add(new RtfColorTableEntry(0, 0, 0));
            colorTable.Add(new RtfColorTableEntry(255, 255, 255));
            colorTable.AddUnique(new RtfColorTableEntry(Fonts.DefaultFontRGB[0], Fonts.DefaultFontRGB[1], Fonts.DefaultFontRGB[2]));

            Collection<XmlElement> fontElements = element.GetDescendants("font");

            foreach (XmlElement fontElement in fontElements)
            {
                colorTable.AddUnique(new RtfColorTableEntry(fontElement));
            }
        }

        protected void constructOldTextProperty(IXmlElement element)
        {
            rawDataElement = element.GetChild("rawHtmlData");

            if (rawDataElement != XmlElement.NULL)
            {
                // HTML document content
                Rtf = rtfStringPrefix + HtmlData.ConvertToPlainTextRtf(rawDataElement) + rtfStringPostfix;
            }
            else
            {
                // old text & field XML format
                Rtf = rtfFromTextElement(element);
            }

            text = "";
        }

        /// <summary>
        /// Return older text + field elements in TX control compatible RTF
        /// </summary>
        private string rtfFromTextElement(IXmlElement element)
        {
            var sb = new StringBuilder(rtfStringPrefix);

            int i = 0;
            while (element.GetChild(i) != XmlElement.NULL)
            {
                if (element.GetChild(i).Name == "field")
                {
                    sb.Append("<<" + element.GetChild(i).GetAttribute("name") + ">>");
                }
                else
                {
                    if (element.GetChild(i).Value != null)
                    {
                        sb.Append(Regex.Replace(element.GetChild(i).Value, "\r\n|\n", @"\par "));
                    }
                }

                i++;
            }

            sb.Append(@"\par " + rtfStringPostfix);
            return sb.ToString();
        }

        public string RtfToXml()
        {
            return (parserXmlString);
        }

        private Collection<IDocumentBlock> makeContents(string xmlString)
        {
            var contents = new Collection<IDocumentBlock>();

            IXmlElement element = new XmlElement(xmlString);

            Collection<XmlElement> blockElements = element.GetChildren();

            foreach (IXmlElement blockElement in blockElements)
            {
                contents.Add(blockFactory.MakeObject(blockElement));
            }

            return contents;
        }

        protected override FieldList getFieldList()
        {
            if (text != "")
            {
                return base.getFieldList();
            }

            var fieldList = new FieldList();

            MatchCollection mc = Regex.Matches(Rtf, @"<<(\w+:[a-z]+|Unknown Field)>>");

            for (int i = 0; i < mc.Count; i++)
            {
                // get display name (such as "Q1:a") from second matched group
                // (the one in parentheses in the regex pattern)
                // make field and add to local list
                fieldList.Add(new Field(mc[i].Groups[1].ToString()));
            }

            return fieldList;
        }

        public override string ToXml()
        {
            var xmlString = new StringBuilder(xmlDocumentStartTag);

            xmlString.Replace("$DOCNAME", XMLStringFormatter.EscapeAttributeText(Name));

            xmlString.Append(xmlDataStartTag);
            foreach (IDocumentBlock block in contents)
            {
                xmlString.Append(block.ToXml());
            }
            xmlString.Append(xmlDataEndTag);

            xmlString.Append(xmlDocumentEndTag);

            return xmlString.ToString();
        }

        public string ToRtf()
        {
            var rtfString = new StringBuilder(rtfProlog);

            rtfString.Append(fontTable.ToRtf());
            rtfString.Append(colorTable.ToRtf());
            rtfString.Append(rtfDefaultTabs);

            foreach (IDocumentBlock block in contents)
            {
                rtfString.Append(block.ToRtf(this));
            }

            rtfString.Append(rtfEnd);

            return rtfString.ToString();
        }

        #region Nested type: NullRtfDocument

        [Serializable]
        private class NullRtfDocument : RtfDocument
        {
            public NullRtfDocument(string name) : base(name)
            {
            }
        }

        #endregion
    }
}