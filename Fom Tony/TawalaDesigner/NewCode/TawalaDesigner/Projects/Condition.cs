// $Workfile: Condition.cs $
// $Revision: 66 $	$Date: 3/20/08 4:36p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using Tawala.Common;
using Tawala.Projects.Fields;
using Tawala.Projects.Parsing;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	/// <summary>
	/// Class to contain a single condition in a Conditions object
	/// </summary>
	[Serializable]
	public class Condition : IConditionComponent
	{
		protected static Factory<Expression> expressionFactory = new Factory<Expression>();

		static Condition()
		{
			expressionFactory.Register("string", "value", typeof(ValueExpression));
			expressionFactory.Register("string", "field", typeof(FieldExpression));

			expressionFactory.Register("equals", typeof(CompoundExpression));
			expressionFactory.Register("doesNotEqual", typeof(CompoundExpression));
			expressionFactory.Register("contains", typeof(CompoundExpression));
			expressionFactory.Register("doesNotContain", typeof(CompoundExpression));
			expressionFactory.Register("beginsWith", typeof(CompoundExpression));
			expressionFactory.Register("endsWith", typeof(CompoundExpression));
			expressionFactory.Register("isLessThan", typeof(CompoundExpression));
			expressionFactory.Register("isLessThanOrEqualTo", typeof(CompoundExpression));
			expressionFactory.Register("isGreaterThan", typeof(CompoundExpression));
			expressionFactory.Register("isGreaterThanOrEqualTo", typeof(CompoundExpression));

			expressionFactory.Register("add", typeof(ArithmeticExpression));
			expressionFactory.Register("sub", typeof(ArithmeticExpression));
			expressionFactory.Register("mul", typeof(ArithmeticExpression));
			expressionFactory.Register("div", typeof(ArithmeticExpression));
		}

		public Condition()
		{
		}

		public Condition(IField field, ComparisonOperator compOp, Expression expression)
		{
			this.field = field;
			this.fieldString = field.FieldName;
			this.compOp = compOp;
			this.expression = expression;
		}

		public Condition(IField field, ComparisonOperator compOp, IChoice choice)
		{
			this.field = field;
			this.fieldString = field.FieldName;
			this.compOp = compOp;
			this.expression = new Expression(choice);
		}

		public Condition(IField field, ComparisonOperator compOp)
		{
			this.field = field;
			this.fieldString = field.FieldName;
			this.compOp = compOp;
			this.expression = new Expression();
		}

		protected Condition(IXmlElement element, IField fieldResolver) : this()
		{
			string fieldName = element.GetAttribute("field");

			if (isVariable(fieldName))
			{
				this.field = new Variable(fieldName);
			}
			else
			{
				this.field = fieldResolver[fieldName];
				if (field == null)
				{
					field = new UnresolvedField(fieldName);
				}
				else
				{
					this.FieldString = field.FieldName;
				}
			}
		}

		protected Condition(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
		{
		}

		protected Condition(IXmlElement element, Process process) : this()
		{
			string fieldName = element.GetAttribute("field");

			if (isVariable(fieldName))
			{
				field = new Variable(fieldName);
			}
			else
			{
				if (process.MappedFields.ContainsKey(fieldName))
				{
					field = process.MappedFields[fieldName];
				}
			}

			if (field == null)
			{
				if (FieldUtil.IsUnknownField(fieldName))
				{
					field = new Variable(FieldUtil.UnknownFieldName);
				}
				else
				{
					if (Project.FieldMapByName.ContainsKey(fieldName))
					{
						field = Project.FieldMapByName[fieldName];
					}
					else
					{
						field = new UnresolvedField(fieldName);
					}
				}
			}

			if (field != null)
			{
				FieldString = field.FieldName;
			}
		}

		private static bool isVariable(string fieldName)
		{
			return FieldUtil.IsVariable(fieldName);
		}

		protected Boolean hasStringOperator()
		{
			if (compOp is HybridOperator)
			{
				return (((HybridOperator)compOp).Op == HybridOperator.Ops.contains ||
						((HybridOperator)compOp).Op == HybridOperator.Ops.doesNotContain ||
						((HybridOperator)compOp).Op == HybridOperator.Ops.beginsWith ||
						((HybridOperator)compOp).Op == HybridOperator.Ops.endsWith ||
						((HybridOperator)compOp).Op == HybridOperator.Ops.equals ||
						((HybridOperator)compOp).Op == HybridOperator.Ops.doesNotEqual);
			}

			if (compOp is StringOperator)
			{
				return (((StringOperator)compOp).Op == StringOperator.Ops.contains ||
						((StringOperator)compOp).Op == StringOperator.Ops.doesNotContain ||
						((StringOperator)compOp).Op == StringOperator.Ops.beginsWith ||
						((StringOperator)compOp).Op == StringOperator.Ops.endsWith ||
						((StringOperator)compOp).Op == StringOperator.Ops.equals ||
						((StringOperator)compOp).Op == StringOperator.Ops.doesNotEqual);
			}

			return false;
		}

		protected Boolean hasNumericOperator()
		{
			if (compOp is HybridOperator)
			{
				return (((HybridOperator)compOp).Op == HybridOperator.Ops.equals ||
						((HybridOperator)compOp).Op == HybridOperator.Ops.doesNotEqual ||
						((HybridOperator)compOp).Op == HybridOperator.Ops.isLessThan ||
						((HybridOperator)compOp).Op == HybridOperator.Ops.isLessThanOrEqualTo ||
						((HybridOperator)compOp).Op == HybridOperator.Ops.isGreaterThan ||
						((HybridOperator)compOp).Op == HybridOperator.Ops.isGreaterThanOrEqualTo);
			}

			if (compOp is ArithmeticOperator)
			{
				return (((ArithmeticOperator)compOp).Op == ArithmeticOperator.Ops.equals ||
						((ArithmeticOperator)compOp).Op == ArithmeticOperator.Ops.doesNotEqual ||
						((ArithmeticOperator)compOp).Op == ArithmeticOperator.Ops.isLessThan ||
						((ArithmeticOperator)compOp).Op == ArithmeticOperator.Ops.isLessThanOrEqualTo ||
						((ArithmeticOperator)compOp).Op == ArithmeticOperator.Ops.isGreaterThan ||
						((ArithmeticOperator)compOp).Op == ArithmeticOperator.Ops.isGreaterThanOrEqualTo);
			}

			return false;
		}

		protected Boolean isNumeric(string expressionString)
		{
			string numericPattern = @"^-?\d*\.?\d+$";

			return (Regex.IsMatch(expressionString, numericPattern));
		}

		protected Boolean isArithmetic(string expressionString)
		{
			if (!expressionString.Contains("+") &&
				!expressionString.Contains("-") &&
				!expressionString.Contains("*") &&
				!expressionString.Contains("/"))
			{
				return false;
			}

			MathParser parser = new MathParser(expressionString);

			if (parser.Balance == 0)
			{
				string parsed = parser.ToXml();
				if (parsed.Contains("invalidExpression"))
				{
					return false;
				}
			}
			else
			{
				return false;
			}

			return true;
		}

		protected Boolean isQuotable(string expressionString)
		{
			if (isNumeric(expressionString) && hasNumericOperator())
			{
				return false;
			}

			if (isArithmetic(expressionString))
			{
				return false;
			}

			if (expression.HasSingleFieldElement)
			{
				return false;
			}

			if (compOp is MCOneOperator || compOp is MCManyOperator)
			{
				return false;
			}

			return true;
		}

		private Boolean expressionRequired()
		{
			if (compOp is HybridOperator)
			{
				HybridOperator hOp = compOp as HybridOperator;

				return (!(hOp.Op == HybridOperator.Ops.isBlank) && !(hOp.Op == HybridOperator.Ops.isNotBlank));
			}
			else if (compOp is MCOneOperator)
			{
				MCOneOperator mcOp = compOp as MCOneOperator;

				return (!(mcOp.Op == MCOneOperator.Ops.mcIsBlank) && !(mcOp.Op == MCOneOperator.Ops.mcIsNotBlank));
			}
			else if (compOp is MCManyOperator)
			{
				MCManyOperator mcOp = compOp as MCManyOperator;

				return (!(mcOp.Op == MCManyOperator.Ops.mcIsBlank) && !(mcOp.Op == MCManyOperator.Ops.mcIsNotBlank));
			}

			return true;
		}

		public override string ToString()
		{
			string expString = "";
			string tempString;

			if (expression != null)
			{
				if (expressionRequired())
				{
					expString = stripAngleBrackets(expression.ToString());

					if (isQuotable(expString))
					{
						expString = "\"" + expString + "\"";
					}
				}
			}

			if (Field != null)
			{
				tempString = Field.ToString() + " " + compOp.Name + " " + expString;
			}
			else
			{
				tempString = fieldString + " " + compOp.Name + " " + expString;
			}

			return tempString.Trim();
		}

		protected string stripAngleBrackets(string inputString)
		{
			return inputString.Replace("<<", "").Replace(">>", "");
		}

		public string ToXml()
		{
			if (expression != null)
			{
				if (!expressionRequired())
				{
					return noExpressionXml();
				}
				else
				{
					if (conditionComparesMcqToLiteralChoice())
					{
						return mcValueXml();
					}
					else
					{
						return fieldOrStringXml();
					}
				}
			}

			return string.Empty;
		}

		private string mcValueXml()
		{
			return string.Format("<{0} field=\"{1}\" value=\"{2}\"/>\r\n",
				compOp.XmlTagName,
				XMLStringFormatter.EscapeAttributeText(Field.ToString()),
				XMLStringFormatter.EscapeAttributeText(stripAngleBrackets(expression.ToString())));
		}

		private string noExpressionXml()
		{
			return string.Format("<{0} field=\"{1}\" />\r\n",
				compOp.XmlTagName,
				XMLStringFormatter.EscapeAttributeText(Field.ToString()));
		}

		private string fieldOrStringXml()
		{
			StringBuilder xmlString = new StringBuilder();

			xmlString.AppendFormat("<{0} field=\"{1}\">\r\n",
				compOp.XmlTagName,
				XMLStringFormatter.EscapeAttributeText(Field.ToString()));

			xmlString.Append(expression.ToXml());
			
			xmlString.AppendFormat("</{0}>\r\n", compOp.XmlTagName);

			return xmlString.ToString();
		}

		private bool conditionComparesMcqToLiteralChoice()
		{
			return (compOp is MCManyOperator || compOp is MCOneOperator) && expression.HasSingleChoiceField;
		}

		protected ComparisonOperator compOp;

		public ComparisonOperator CompOp
		{
			get
			{
				return compOp;
			}
			set
			{
				compOp = value;
			}
		}

		protected string fieldString;

		public string FieldString
		{
			get
			{
				return fieldString;
			}
			set
			{
				fieldString = value;
			}
		}

		[NonSerialized]
		protected IField field;

		public IField Field
		{
			get
			{
				if (field is UnresolvedField)
				{
					field = FieldUtil.ResolveFormField(field);
					this.FieldString = field.FieldName;
				}

				return field;
			}
			set
			{
				field = value;
			}
		}

		protected Expression expression;

		public Expression Expression
		{
			get
			{
				return expression;
			}
		}

		public string ExpressionString
		{
			get
			{
				if (expression != null)
				{
					return stripAngleBrackets(expression.ToString());
				}

				return "";
			}
		}

		private FieldSerializationInfo fieldSerializationInfo = null;

		[OnSerializing]
		private void onSerializing(StreamingContext context)
		{
			fieldSerializationInfo = new FieldSerializationInfo(Field);
		}

		[OnDeserialized]
		private void onDeserialized(StreamingContext context)
		{
			this.Field = fieldSerializationInfo.Deserialized();
			fieldSerializationInfo = null;
		}
    }

	[Serializable]
	public class FibCondition : Condition
	{
		public FibCondition(IXmlElement element, IField fieldResolver)
			: base(element, fieldResolver)
		{
			this.compOp = HybridOperator.List[element.Name];
			this.expression = expressionFactory.MakeObject(element, fieldResolver);
		}

		public FibCondition(IXmlElement element, string processName)
			: base(element, processName)
		{
			this.compOp = HybridOperator.List[element.Name];
			this.expression = expressionFactory.MakeObject(element, processName);
		}

		public FibCondition(IXmlElement element, Process process)
			: base(element, process)
		{
			this.compOp = HybridOperator.List[element.Name];
			this.expression = expressionFactory.MakeObject(element, process);
		}
	}

	[Serializable]
	public class FibNoExpressionCondition : Condition
	{
		public FibNoExpressionCondition(IXmlElement element, IField fieldResolver)
			: base(element, fieldResolver)
		{
			this.compOp = HybridOperator.List[element.Name];
			this.expression = new Expression();
		}

		public FibNoExpressionCondition(IXmlElement element, string processName)
			: base(element, processName)
		{
			this.compOp = HybridOperator.List[element.Name];
			this.expression = new Expression();
		}

		public FibNoExpressionCondition(IXmlElement element, Process process)
			: base(element, process)
		{
			this.compOp = HybridOperator.List[element.Name];
			this.expression = new Expression();
		}
	}

	[Serializable]
	public class MCValueCondition : Condition
	{
		public MCValueCondition(IXmlElement element, IField fieldResolver) : base(element, fieldResolver)
		{
			construct(element);
		}

		public MCValueCondition(IXmlElement element, string processName)
			: base(element, processName)
		{
			construct(element);
		}

		public MCValueCondition(IXmlElement element, Process process)
			: base(element, process)
		{
			construct(element);
		}

		private void construct(IXmlElement element)
		{
			this.compOp = MCManyOperator.List[element.Name];
			ChoiceField choiceField = new ChoiceField(element.GetAttribute("value"));
			this.expression = new Expression(choiceField);
		}
	}

	[Serializable]
	public class MCFieldCondition : Condition
	{
		public MCFieldCondition(IXmlElement element, IField fieldResolver) : base(element, fieldResolver)
		{
			this.compOp = MCManyOperator.List[element.Name];
			this.expression = expressionFactory.MakeObject(element.GetChild("string"), fieldResolver);
		}

		public MCFieldCondition(IXmlElement element, string processName)
			: base(element, processName)
		{
			this.compOp = MCManyOperator.List[element.Name];
			this.expression = expressionFactory.MakeObject(element.GetChild("string"), processName);
		}

		public MCFieldCondition(IXmlElement element, Process process)
			: base(element, process)
		{
			this.compOp = MCManyOperator.List[element.Name];
			this.expression = expressionFactory.MakeObject(element.GetChild("string"), process);
		}
	}

	[Serializable]
	public class MCFieldNoExpressionCondition : Condition
	{
		public MCFieldNoExpressionCondition(IXmlElement element, IField fieldResolver)
			: base(element, fieldResolver)
		{
			construct(element);
		}

		public MCFieldNoExpressionCondition(IXmlElement element, string processName)
			: base(element, processName)
		{
			construct(element);
		}

		public MCFieldNoExpressionCondition(IXmlElement element, Process process)
			: base(element, process)
		{
			construct(element);
		}

		private void construct(IXmlElement element)
		{
			this.expression = new Expression();

			IMcqItem mcField = field as IMcqItem;
			if (field is RecordField)
			{
				mcField = ((RecordField)field).ReferenceField as IMcqItem;
			}

			if (mcField != null)
			{
				if (mcField.SelectOnlyOne)
				{
					this.compOp = MCOneOperator.List[element.Name];
				}
				else
				{
					this.compOp = MCManyOperator.List[element.Name];
				}
			}
			else
			{
				Debug.Assert(false, "Expected field to be of type IMcqItem");
				this.compOp = MCManyOperator.List[element.Name];
			}
		}
	}

}

