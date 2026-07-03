// $Workfile: FieldOrLiteral.cs $
// $Revision: 17 $	$Date: 11/16/06 9:20a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Runtime.Serialization;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class FieldOrLiteral
	{
		public FieldOrLiteral()
		{
		}

		/// <summary>
		/// constructor that takes string text and type (literal or field label) arguments
		/// </summary>
		public FieldOrLiteral(String initialText, StringType initialType)
		{
			text = initialText;
			Type = initialType;

			expression = new Expression(initialText);

			if (Type == StringType.field)
			{
				expression = new Expression("<<" + initialText + ">>");
			}
			else if (Type == StringType.compound)
			{
				expression = new Expression(initialText);
			}

		}

		/// <summary>
		/// constructor that takes string text and type (literal or field label) arguments
		/// </summary>
		public FieldOrLiteral(String initialText, StringType initialType, IField fieldResolver)
		{
			text = initialText;
			Type = initialType;

			expression = new Expression(initialText);

			if (Type == StringType.field)
			{
				expression = new Expression("<<" + initialText + ">>", fieldResolver);
			}
			else if (Type == StringType.compound)
			{
				expression = new Expression(initialText, fieldResolver);
			}
		}

		/// <summary>
		/// constructor that takes RecordField argument
		/// </summary>
		public FieldOrLiteral(RecordField recordField)
		{
			expression = new Expression(recordField);
			type = StringType.field;
		}

		/// <summary>
		/// constructor that takes QualifiedFieldList argument
		/// </summary>
		public FieldOrLiteral(QualifiedFieldList qualifiedField)
		{
			expression = new Expression(qualifiedField);
			type = StringType.field;
		}

		/// <summary>
		/// constructor that takes IXmlElement argument
		/// </summary>
		public FieldOrLiteral(IXmlElement xmlElement)
		{
		}

		/// <summary>
		/// Type of string (literal or field label)
		/// </summary>
		public enum StringType
		{
			literal,	// expression is a single literal
			field,		// expression is a single field
			compound	// expression is a mix of literals and fields
		}

		private StringType type = StringType.literal;

		public StringType Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;

				if (type == StringType.field)
				{
					expression = new Expression("<<" + text + ">>");
				}
				else if (Type == StringType.compound)
				{
					expression = new Expression(text);
				}

			}
		}

		/// <summary>
		/// The unformatted text of the string
		/// </summary>
		private string text = "";

		public string Text
		{
			get
			{
				if (Type == StringType.field || Type == StringType.compound)
				{
					return expression.ToString().Replace("<<", "").Replace(">>", "");
				}

				return text;
			}
			set
			{
				text = value;

				if (Type == StringType.field)
				{
					expression = new Expression("<<" + text + ">>");
				}
				else if (Type == StringType.compound)
				{
					expression = new Expression(text);
				}

			}
		}

		public string RawText
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		/// <summary>
		/// Statement's expression component.
		/// </summary>
		[OptionalField()]
		private Expression expression = new Expression("");

		public Expression Expression
		{
			get
			{
				return expression;
			}
			set
			{
				expression = value;
			}
		}

		/// <summary>
		/// property that discloses if this is a Field OR a literal string containing a numeric
		/// </summary>
		/// <remarks>
		/// this is used to determine when to put quotation marks around the value
		/// when it's displayed in the Process window
		/// </remarks>
		public bool LiteralIsNumeric
		{
			get
			{
				bool numeric = true;

				// check for non-numeric characters
				char[] array = text.ToCharArray();
				for (int j = 0; j < text.Length; j++)
				{
					// Note: assumes non-international decimal point
					if (!Char.IsNumber(array[j]) && array[j] != '.' && array[j] != '-')
					{
						numeric = false;
						break;
					}
				}
				return numeric;
			}
		}

		public override string ToString()
		{
			string retString = Text;

			if (type == StringType.literal)
			{
				if (!LiteralIsNumeric)
				{
					// enclose the string in quotes
					retString = "\"" + Text + "\"";
				}
			}

			return retString;
		}

		public string ToXml()
		{
			return expression.ToXml();
		}

	}
}
