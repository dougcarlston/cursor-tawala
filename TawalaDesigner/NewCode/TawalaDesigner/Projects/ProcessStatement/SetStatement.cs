// $Workfile: SetStatement.cs $
// $Revision: 53 $	$Date: 1/18/08 11:40a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Diagnostics;
using Tawala.Common;
using Tawala.Projects.Forms;
using Tawala.Projects.Parsing;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements a Set statement in the Process
	/// </summary>
	[Serializable]
	public class SetStatement : Tawala.Projects.ProcessStatement
	{
		protected static Factory<Expression> expressionFactory = new Factory<Expression>();

		static SetStatement()
		{
			expressionFactory.Register("set", typeof(CompoundExpression));

			expressionFactory.Register("add", typeof(ArithmeticExpression));
			expressionFactory.Register("sub", typeof(ArithmeticExpression));
			expressionFactory.Register("mul", typeof(ArithmeticExpression));
			expressionFactory.Register("div", typeof(ArithmeticExpression));
		}

		public SetStatement()
		{
			name = "Set";
		}

		public SetStatement(Process process) : this()
		{
			this.process = process;
		}

		public SetStatement(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
		{
		}

		public SetStatement(IXmlElement element, Process process) : this(process)
		{
			string fieldName = element.GetAttribute("field");

			IAssignableField assignable = Project.FieldMapByName[fieldName] as IAssignableField;

			if (isVariable(fieldName))
			{
				this.variable = new Variable(fieldName);
				addVariableToProcess();
			}
			else if (assignable != null)
			{
				variable = assignable;
			}
			else
			{
				this.variable = new AssignableField(fieldName);
			}
			
			this.treatArithmeticAsText = Convert.ToBoolean(element.GetAttribute("arithmeticAsText"));
			this.expression = expressionFactory.MakeObject(element, process);
		}

		private bool isVariable(string fieldString)
		{
			return FieldUtil.IsVariable(fieldString);
		}

		private bool isVariable(IPaletteField field)
		{
			return (field is Variable);
		}

		private void addVariableToProcess()
		{
			if (process != null)
			{
				if (isVariable(variable))
				{
					process.Variables.AddUnique(variable as Variable);
					process.MappedFields.Fields.Add(variable);

					// The following line was commented out to improve the performance of Project open.
					// It appears to be completely unnecessary. - SB 01/18/2008
					//process.MappedFields.Map();
				}
			}
		}

		/// <summary>
		/// The Process containing this statement
		/// </summary>
		private Process process;

		/// <summary>
		/// The statement's field component (variable).
		/// </summary>
		[NonSerialized]
		IAssignableField variable = new Variable();

		public IAssignableField Variable
		{
			get { return variable; }
			
			set
			{
				variable = value;
				if (variable is Variable)
				{
					addVariableToProcess();
				}
				else if (FieldUtil.IsRecordField(value.QualifiedFieldName))
				{
					handleRecordAndFormQualifiedVariable(value.QualifiedFieldName);
				}
			}
		}

		private static void handleRecordAndFormQualifiedVariable(string qualifiedFieldName)
		{
			IForm form = Project.Current.GetForm(FieldUtil.GetFormName(qualifiedFieldName));
			
			if (form != null)
			{
				string unqualifiedName = FieldUtil.GetFieldName(qualifiedFieldName);
				if (!isExistingField(unqualifiedName, form))
				{
					addVariableToFormConnectedProcess(unqualifiedName, form);
				}
			}
		}

		private static bool isExistingField(string unqualifiedName, IForm form)
		{
			foreach (IField field in form.GetAllFields())
			{
				if (field.FieldName.Equals(unqualifiedName))
				{
					return true;
				}
			}
			return false;
		}

		private static void addVariableToFormConnectedProcess(string unqualifiedName, IForm form)
		{
			if (form.ConnectedProcess != null)
			{
				// this causes the variable to shows up as a record field in Fields Palette
				form.ConnectedProcess.Variables.AddUnique(new Variable(unqualifiedName));
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

		private bool treatArithmeticAsText = false;

		public bool TreatArithmeticAsText
		{
			get { return treatArithmeticAsText; }
			set { treatArithmeticAsText = value; }
		}

		public FormList ConnectedForms
		{
			get
			{
				return (process != null ? Project.Current.GetFormList(process.Name) : new FormList());
			}
		}

		/// <summary>
		/// checks for any arithmetic operators in the expressionString
		/// </summary>
		public Boolean IsArithmetic()
		{
			return Regex.IsMatch(expression.ToString(), @"\+|\-|\*|/");
		}

		/// <summary>
		/// Provide statement in plain text form.
		/// </summary>
		/// <remarks>
		/// 
		/// Generally speaking, expressions are displayed without quotation marks if
		/// the are numeric in nature (single digits or arithmetic expression). Since
		/// the type (number or string) of a field is unknown, an expression is assumed
		/// to be arithmetic if it contains a +, -, * or / operator.
		/// 
		///	Description					Expression				Display
		///	===========					==========				=======
		///	Single numeric values		2						2
		///								2.3						2.3
		///								-2						-2
		///	Arithmetic expressions		2 + 3					2 + 3
		///								<<Q1:a>> + 3			<<Q1:a>> + 3
		///								<<Q1:a>> + <<Q1:b>>		<<Q1:a>> + <<Q1:b>>
		/// 
		/// 
		/// Non-numeric expressions are considered strings and are displayed in
		/// quotation marks. Also, if an aritmetic expression is invalid (unbalanced
		/// parentheses, bad form, etc., it's treated as a string.
		/// 
		///	Description					Expression				Display
		///	===========					==========				=======
		/// Non-numeric characters		Fred					"Fred"
		///		"			"
		///		mixed with fields		Fred <<Last Name>>		"Fred <<Last Name>>"
		///								2 <<Field>>				"2 <<Field>>"
		///								<<Field1>> <<Field2>>	"<<Field1>> <<Field2>>"
		/// Invalid expression			((2 + 7) / 3			"((2 + 7) / 3"
		/// 
		/// 
		/// A single Field with no other text is displayed without quotes and without
		/// angle brackets. Multiple concatenated fields with no other literal characters
		/// are displayed with the angle brackets to keep them distinguishable.
		/// 
		///	Description					Expression				Display
		///	===========					==========				=======
		///	Single field				<<Field1>>				Field1
		///	Multiple fields				<<Field1>><<Field2>>	<<Field1>><<Field2>>
		/// 
		/// </remarks>
		public override string ToString()
		{
			string expString = expression.ToString();

			// if the expression is arithmetic, we don't put quotes around it
			if (IsArithmetic() && !treatArithmeticAsText)
			{
				// unless it's an invalid expression (which we treat as a string)
				MathParser parser = new MathParser(expression.ToString());
				if (parser.Balance == 0)
				{
					string parsed = parser.ToXml();
					if (parsed.Contains("invalidExpression"))
					{
						expString = "\"" + expression.ToString() + "\"";
					}
				}
				else
				{
					// unbalanced parentheses
					expString = "\"" + expression.ToString() + "\"";
				}
			}
			else
			{
				// check to see if it's a single numeric value
				bool nonNumeric = false;
				char[] array = expression.ToString().ToCharArray();
				for (int j = 0; j < expression.ToString().Length; j++)
				{
					// Note: a period for a decimal point is not international
					if (!Char.IsNumber(array[j]) && array[j] != '.')
					{
						nonNumeric = true;
						break;
					}
				}

				// if it's not strictly numeric
				bool concatenatedFields = true;
				if (nonNumeric)
				{
					// see if it consists of one or more fields
					string pattern = @"<<.+?>>|[\x21-\x3b\x3d\x3f-\x7e\s]+|<+|>+";
					MatchCollection mc = Regex.Matches(expression.ToString(), pattern);

					for (int i = 0; i < mc.Count; i++)
					{
						// with no other text characters
						if (!Regex.Match(mc[i].ToString(), "<<.+?>>").Success)
						{
							concatenatedFields = false;
							break;
						}
					}

					// if it's a single field
					if (concatenatedFields && mc.Count == 1)
					{
						// strip off the angle brackets
						expString = expression.ToString().Substring(2, expression.ToString().Length - 4);
					}
				}

				// unless the expression is a simple number or only fields
				if (nonNumeric && !concatenatedFields)
				{
					// put it in quotes
					expString = "\"" + expression.ToString() + "\"";
				}
			}

			return Name + " " + variable.QualifiedFieldName + " to " + expString;
		}

		private const string xmlSetStartTag = "<set field=\"{0}\" arithmeticAsText=\"{1}\">\r\n";
		private const string xmlSetEndTag = "</set>";
		private const string xmlStringValueTag = "<string value=\"$VALUE\"/>\r\n";
		private const string xmlStringFieldTag = "<string field=\"$FIELD\"/>\r\n";

		public override string ToXml()
		{
			StringBuilder xmlString = new StringBuilder();

			xmlString.AppendFormat(xmlSetStartTag,
				XMLStringFormatter.EscapeAttributeText(variable.QualifiedFieldName),
				treatArithmeticAsText ? "true" : "false");

			// add the expression
			if (expression.ToString().Length > 0)
			{
				// look for any arithmetic operators
				if (IsArithmetic() && !treatArithmeticAsText)
				{
					// try parsing as an arithmetic expression
					MathParser parser = new MathParser(expression.ToString());
					string parsed = parser.ToXml();

					// if it just don't look good
					if (parser.Balance != 0 || parsed.Contains("invalidExpression"))
					{
						// we'll write it out as a string (removing any special characters)
						xmlString.Append(xmlStringValueTag);
						xmlString.Replace("$VALUE", XMLStringFormatter.EscapeAttributeText(expression.ToString()));
					}
					else
					{
						// looks like a good expression, ship it!
						xmlString.Append(parsed);
					}
				}
				else
				{
					// here we have a string with no arithmetic operators
					// split out any fields from the rest of the text
					string pattern = @"<<.+?>>|[\x21-\x3b\x3d\x3f-\x7e\s]+|<+|>+";
					MatchCollection mc = Regex.Matches(expression.ToString(), pattern);

					for (int i = 0; i < mc.Count; i++)
					{
						string token = mc[i].ToString();
						if (Regex.Match(token, "<<.+?>>").Success)
						{
							// field
							xmlString.Append(xmlStringFieldTag);
							xmlString.Replace("$FIELD", XMLStringFormatter.EscapeAttributeText(token.Substring(2, token.Length - 4)));
						}
						else
						{
							// literal string (removing any special characters)
							xmlString.Append(xmlStringValueTag);
							xmlString.Replace("$VALUE", XMLStringFormatter.EscapeAttributeText(token));
						}
					}
				}
			}

			// and the end tag
			xmlString.Append(xmlSetEndTag);

			return xmlString.ToString();
		}

        [OnSerializing]
        private void onSerializing(StreamingContext context)
        {
            serializationInfo = new SerializationInfo(this);
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            serializationInfo.Deserialized(this);
            serializationInfo = null;
        }

        private SerializationInfo serializationInfo = null;

		[Serializable]
		class SerializationInfo
		{
			private int fieldId;

			public SerializationInfo(SetStatement statement)
			{
				fieldId = statement.Variable.Id;
			}

			public void Deserialized(SetStatement statement)
			{
				Debug.Assert(Project.FieldMapById.ContainsKey(fieldId));

				if (Project.FieldMapById.ContainsKey(fieldId))
				{
					statement.Variable = Project.FieldMapById[fieldId] as IAssignableField;
					Debug.Assert(statement.Variable != null);
				}
			}
		}
	}
}
