// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Common;
using Tawala.XmlSupport;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements a Show URL statement in the Process.  
    /// </summary>
    [Serializable]
    public class ShowUrlStatement : ShowStatement
    {
        private string urlString;

        public ShowUrlStatement(string urlString)
        {
            name = "Show";
            this.urlString = urlString;
        }

        /// <summary>
        /// Constructs a ShowUrlStatement from an XML element beginning with a &lt;show&gt; tag
        /// </summary>
        public ShowUrlStatement(IXmlElement element)
        {
            Collection<XmlElement> urlElements = element.GetDescendants("url");

            if (urlElements.Count > 0)
            {
                if (urlElements[0].HasChild("string"))
                {
                    IXmlElement stringElement = urlElements[0].GetChild("string");

                    urlString = stringElement.GetAttribute("value");
                }
            }
        }

        public ShowUrlStatement(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
        {
        }

        public ShowUrlStatement(IXmlElement element, Process process) : this(element)
        {
        }

        public String Url { get { return urlString; } set { urlString = value; } }

        public override Type GetStatementType()
        {
            return typeof(ShowStatement);
        }

        public override string ToString()
        {
            return (Name + " URL " + urlString);
        }

        public override string ToXml()
        {
            var xmlString = new StringBuilder();

            xmlString.Append("<show>");
            xmlString.Append("<url>");
            xmlString.AppendFormat("<string value=\"{0}\"/>", XMLStringFormatter.EscapeAttributeText(urlString));
            xmlString.Append("</url>");
            xmlString.Append("</show>");

            return xmlString.ToString();
        }
    }
}