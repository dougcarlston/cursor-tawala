// $Workfile: UrlExpression.cs $
// $Revision: 4 $	$Date: 3/14/08 2:20p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements expressions used in SET statements
	/// </summary>

	[Serializable]
	public class UrlExpression
	{
		protected static string stringPattern;
		protected static string fieldPattern;
		protected static string stringFieldPattern;
		private const int FIRST_CAPTURED_GROUP = 1;
		private const int SECOND_CAPTURED_GROUP = 2;

		static UrlExpression()
		{
			// matches fields (such as "<<Q1:a>>" or "<<Name>>")
			fieldPattern = @"<<((?:(?!>>).)*)>>";

			// matches anything that is not a field
			stringPattern = @"((?:(?!<<).)+)";

			// combine field and string patterns into one alternation
			stringFieldPattern = fieldPattern + "|" + stringPattern;
		}

		//public UrlExpression(string expressionString)
		//{
		//    construct(expressionString, getCurrentFields());
		//}

		public UrlExpression(string expressionString)
		{
			elements = new ExpressionElementList();

			MatchCollection matches = Regex.Matches(expressionString, stringFieldPattern);

			foreach (Match match in matches)
			{
				if (isFieldPattern(match))
				{
					string fieldName = match.Groups[FIRST_CAPTURED_GROUP].Value;
					IField field = getCurrentFields()[fieldName];

					if (field != null)
					{
						elements.Add(new FieldElement(field));
					}
					else
					{
						string qualifiedFieldName = fieldName;

						elements.Add(new FieldElement(new UnresolvedField(qualifiedFieldName)));
					}
				}
				else if (isStringPattern(match))
				{
					elements.Add(new StringElement(match.Groups[SECOND_CAPTURED_GROUP].Value));
				}
			}
		}

		/// <summary>
		/// Constructs a UrlExpression object from a &lt;url&gt; element
		/// </summary>
		public UrlExpression(IXmlElement element)
		{
			elements = new ExpressionElementList();

			foreach (XmlElement childElement in element.GetChildren())
			{
				if (childElement.Name == "string")
				{
					elements.Add(new StringElement(childElement.GetAttribute("value")));
				}
				else if (childElement.Name == "field")
				{
					IField field = getCurrentFields()[childElement.GetAttribute("name")];

					if (field != null)
					{
						elements.Add(new FieldElement(field));
					}
					else
					{
						string qualifiedFieldName = childElement.GetAttribute("name").Replace("<<", "").Replace(">>", "");

						elements.Add(new FieldElement(new UnresolvedField(qualifiedFieldName)));
					}
				}
			}
		}

		protected void construct(string expressionString, IField fieldResolver)
		{
			elements = new ExpressionElementList();

			MatchCollection matches = Regex.Matches(expressionString, stringFieldPattern);

			foreach (Match match in matches)
			{
				if (isFieldPattern(match))
				{
					string fieldName = match.Groups[FIRST_CAPTURED_GROUP].Value;
					IField field = fieldResolver[fieldName];

					if (field != null)
					{
						elements.Add(new FieldElement(field));
					}
				}
				else if (isStringPattern(match))
				{
					elements.Add(new StringElement(match.Groups[SECOND_CAPTURED_GROUP].Value));
				}
			}

		}

		private static bool isFieldPattern(Match m)
		{
			return Regex.IsMatch(m.Value, fieldPattern);
		}

		private static bool isStringPattern(Match match)
		{
			return Regex.IsMatch(match.Value, stringPattern);
		}

		private FieldList getCurrentFields()
		{
			FieldList fieldList = new FieldList();

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

		/// <summary>
		/// List of expression elements
		/// </summary>
		private ExpressionElementList elements;

		public ExpressionElementList Elements
		{
			get
			{
				return elements;
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			if (elements != null)
			{
				foreach (ExpressionElement e in elements)
				{
					sb.Append(e.ToString());
				}
			}

			return sb.ToString();
		}


		protected const string xmlStringValueTag = "<string value=\"{0}\"/>\r\n";
		protected const string xmlFieldValueTag = "<field name=\"{0}\"/>\r\n";

		public virtual string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();

			if (ToString().Length > 0)
			{
				MatchCollection matches = Regex.Matches(ToString(), stringFieldPattern);

				for (int i = 0; i < matches.Count; i++)
				{
					string token = matches[i].ToString();
					
					if (Regex.Match(token, fieldPattern).Success)
					{
						xmlString.AppendFormat(xmlFieldValueTag, XMLStringFormatter.EscapeAttributeText(token.Substring(2, token.Length - 4)));
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
