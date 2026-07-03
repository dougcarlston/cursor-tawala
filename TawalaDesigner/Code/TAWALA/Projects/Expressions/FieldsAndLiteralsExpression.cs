// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Projects.Fields;
using Tawala.XmlSupport;

namespace Tawala.Projects.Expressions
{
    /// <summary>
    /// Implements expressions used in SET statements
    /// </summary>
    [Serializable]
    public sealed class FieldsAndLiteralsExpression
    {
        private const int FIRST_CAPTURED_GROUP = 1;
        private const int SECOND_CAPTURED_GROUP = 2;
        private const string xmlFieldValueTag = "<field name=\"{0}\"/>\r\n";
        private const string xmlStringValueTag = "<string value=\"{0}\"/>\r\n";
        private static readonly string fieldPattern;
        private static readonly string stringFieldPattern;
        private static readonly string stringPattern;

        static FieldsAndLiteralsExpression()
        {
            // matches fields (such as "<<Q1:a>>" or "<<Name>>")
            fieldPattern = @"<<((?:(?!>>).)*)>>";

            // matches anything that is not a field
            stringPattern = @"((?:(?!<<).)+)";

            // combine field and string patterns into one alternation
            stringFieldPattern = fieldPattern + "|" + stringPattern;
        }

        public FieldsAndLiteralsExpression(string expressionString)
        {
            Elements = new ExpressionElementList();

            MatchCollection matches = Regex.Matches(expressionString, stringFieldPattern);

            foreach (Match match in matches)
            {
                if (isFieldPattern(match))
                {
                    string fieldName = match.Groups[FIRST_CAPTURED_GROUP].Value;
                    IField field = getCurrentFields()[fieldName];

                    if (field != null)
                    {
                        Elements.Add(new FieldElement(field));
                    }
                    else
                    {
                        string qualifiedFieldName = fieldName;

                        Elements.Add(new FieldElement(new UnresolvedField(qualifiedFieldName)));
                    }
                }
                else if (isStringPattern(match))
                {
                    Elements.Add(new StringElement(match.Groups[SECOND_CAPTURED_GROUP].Value));
                }
            }
        }

        /// <summary>
        /// Constructs a FieldsAndLiteralsExpression object from a &lt;url&gt; element
        /// </summary>
        public FieldsAndLiteralsExpression(IXmlElement element)
        {
            Elements = new ExpressionElementList();

            foreach (XmlElement childElement in element.GetChildren())
            {
                if (childElement.Name == "string")
                {
                    if (childElement.HasAttribute("value"))
                    {
                        Elements.Add(new StringElement(childElement.GetAttribute("value")));
                    }
                    else if (childElement.HasAttribute("field"))
                    {
                        createFieldElement(childElement, "field");
                    }
                }
                else if (childElement.Name == "field")
                {
                    createFieldElement(childElement, "name");
                }
            }
        }

        private void createFieldElement(IXmlElement childElement, string attributeName)
        {
            var field = getCurrentFields()[childElement.GetAttribute(attributeName)];

            if (field != null)
            {
                Elements.Add(new FieldElement(field));
            }
            else
            {
                var qualifiedFieldName = childElement.GetAttribute(attributeName).Replace("<<", "").Replace(">>", "");

                Elements.Add(new FieldElement(new UnresolvedField(qualifiedFieldName)));
            }
        }

        public ExpressionElementList Elements { get; private set; }

        private static bool isFieldPattern(Match m)
        {
            return Regex.IsMatch(m.Value, fieldPattern);
        }

        private static bool isStringPattern(Match match)
        {
            return Regex.IsMatch(match.Value, stringPattern);
        }

        private static FieldList getCurrentFields()
        {
            var fieldList = new FieldList();

            foreach (IForm form in Project.Current.FormList)
            {
                foreach (IField field in form.GetFormItemFields())
                {
                    fieldList.Add(field);
                }
            }

            foreach (IField var in Project.Current.AllVariables)
            {
                fieldList.Add(var);
            }

            return fieldList;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Elements != null)
            {
                foreach (ExpressionElement e in Elements)
                {
                    sb.Append(e.ToString());
                }
            }

            return sb.ToString();
        }

        public string ToXml()
        {
            var xmlString = new StringBuilder();

            if (ToString().Length > 0)
            {
                MatchCollection matches = Regex.Matches(ToString(), stringFieldPattern);

                for (int i = 0; i < matches.Count; i++)
                {
                    string token = matches[i].ToString();

                    if (Regex.Match(token, fieldPattern).Success)
                    {
                        xmlString.AppendFormat(xmlFieldValueTag,
                                               XMLStringFormatter.EscapeAttributeText(token.Substring(2, token.Length - 4)));
                    }
                    else
                    {
                        xmlString.AppendFormat(xmlStringValueTag, XMLStringFormatter.EscapeAttributeText(token));
                    }
                }
            }

            return xmlString.ToString();
        }
    }
}