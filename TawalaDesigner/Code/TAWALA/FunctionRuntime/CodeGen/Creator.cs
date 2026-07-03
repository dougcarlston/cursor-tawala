// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Globalization;
using System.Xml.XPath;

namespace Tawala.Functions.Runtime.CodeGen
{
    internal abstract class Creator
    {
        private static readonly TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

        protected static string makeClassName(XPathNavigator navElement)
        {
            string id = navElement.GetAttribute("id", "");
            return ToTitleCase(CleanIdentifier(id)).Replace("_", "");
        }

        protected static string CleanIdentifier(string name)
        {
            return name.Replace("-", "_").Replace(" ", "_");
        }

        protected static string ToTitleCase(string text)
        {
            return textInfo.ToTitleCase(text);
        }

        internal static string GetDescription(XPathNavigator parent)
        {
            XPathNavigator child = parent.SelectSingleNode("child::description");
            return child != null ? child.Value : string.Empty;
        }
    }
}