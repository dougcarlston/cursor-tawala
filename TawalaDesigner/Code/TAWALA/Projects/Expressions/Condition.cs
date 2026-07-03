// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Parsing;
using Tawala.Projects.Expressions;
using Tawala.Projects.Factories;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
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
        protected ComparisonOperator compOp;
        protected Expression expression;

        [NonSerialized]
        protected IField field;

        private FieldSerializationInfo fieldSerializationInfo;
        protected string fieldString;

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
            fieldString = field.FieldName;
            this.compOp = compOp;
            this.expression = expression;
        }

        public Condition(IField field, ComparisonOperator compOp, Choice choice)
        {
            this.field = field;
            fieldString = field.FieldName;
            this.compOp = compOp;
            expression = new Expression(choice);
        }

        public Condition(IField field, ComparisonOperator compOp)
        {
            this.field = field;
            fieldString = field.FieldName;
            this.compOp = compOp;
            expression = new Expression();
        }

        protected Condition(IXmlElement element, IField fieldResolver) : this()
        {
            string fieldName = element.GetAttribute("field");

            if (isVariable(fieldName))
            {
                field = new Variable(fieldName, false);
            }
            else
            {
                field = fieldResolver[fieldName];
                if (field == null)
                {
                    field = new UnresolvedField(fieldName);
                }
                else
                {
                    FieldString = field.FieldName;
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
                else
                {
                    Log.LogInfo("(IXmlElement, Process): Couldn't resolve field name '{0}' in process '{1}' using process.MappedFields",
                                fieldName, process.Name);
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
            else
            {
                Log.LogError("(IXmlElement, Process): Couldn't resolve field name '{0}' in process '{1}'", fieldName, process.Name);
            }
        }

        public ComparisonOperator CompOp { get { return compOp; } set { compOp = value; } }

        public string FieldString { get { return fieldString; } set { fieldString = value; } }

        public IField Field
        {
            get
            {
                if (field is UnresolvedField)
                {
                    field = FieldUtil.ResolveFormField(field);
                    FieldString = field.FieldName;
                }

                return field;
            }
            set { field = value; }
        }

        public Expression Expression { get { return expression; } }

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

        #region IConditionComponent Members

        public bool IsValid()
        {
            if (Field == null && string.IsNullOrEmpty(fieldString))
            {
                return false;
            }

            if (compOp == null)
            {
                return false;
            }

            if (expressionRequired() && expression == null)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            if (compOp == null)
            {
                return string.Empty;
            }

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
                tempString = Field + " " + compOp.Name + " " + expString;
            }
            else
            {
                tempString = fieldString + " " + compOp.Name + " " + expString;
            }

            return tempString.Trim();
        }

        #endregion

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

        protected static Boolean isNumeric(string expressionString)
        {
            const string numericPattern = @"^-?\d*\.?\d+$";

            return (Regex.IsMatch(expressionString, numericPattern));
        }

        protected static Boolean isArithmetic(string expressionString)
        {
            if (!expressionString.Contains("+") && !expressionString.Contains("-") && !expressionString.Contains("*") &&
                !expressionString.Contains("/"))
            {
                return false;
            }

            var parser = new MathParser(expressionString);

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
                var hOp = compOp as HybridOperator;

                return (!(hOp.Op == HybridOperator.Ops.isBlank) && !(hOp.Op == HybridOperator.Ops.isNotBlank));
            }
            if (compOp is MCOneOperator)
            {
                var mcOp = compOp as MCOneOperator;

                return (!(mcOp.Op == MCOneOperator.Ops.mcIsBlank) && !(mcOp.Op == MCOneOperator.Ops.mcIsNotBlank));
            }
            if (compOp is MCManyOperator)
            {
                var mcOp = compOp as MCManyOperator;

                return (!(mcOp.Op == MCManyOperator.Ops.mcIsBlank) && !(mcOp.Op == MCManyOperator.Ops.mcIsNotBlank));
            }

            return true;
        }

        protected static string stripAngleBrackets(string inputString)
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

                if (conditionComparesMcqToLiteralChoice())
                {
                    return mcValueXml();
                }

                return fieldOrStringXml();
            }

            return string.Empty;
        }

        private string mcValueXml()
        {
            insureValidComparisonOperator();

            return string.Format("<{0} field=\"{1}\" value=\"{2}\"/>\r\n", compOp.XmlTagName,
                                 XMLStringFormatter.EscapeAttributeText(Field.ToString()),
                                 XMLStringFormatter.EscapeAttributeText(stripAngleBrackets(expression.ToString())));
        }

        private void insureValidComparisonOperator()
        {
            var mcField = field as IMcqItem;
            if (field is RecordField)
            {
                mcField = ((RecordField)field).ReferenceField as IMcqItem;
            }

            if (mcField != null)
            {
                if (operatorTypesDoNotMatch(mcField))
                {
                    if (compOp == MCManyOperator.List[MCManyOperator.Ops.mcContains])
                    {
                        compOp = MCOneOperator.List[MCOneOperator.Ops.mcEquals];
                    }
                    if (compOp == MCManyOperator.List[MCManyOperator.Ops.mcDoesNotContain])
                    {
                        compOp = MCOneOperator.List[MCOneOperator.Ops.mcDoesNotEqual];
                    }
                }
            }
        }

        private bool operatorTypesDoNotMatch(IMcqItem mcField)
        {
            return mcField.SelectOnlyOne && compOp is MCManyOperator;
        }

        private string noExpressionXml()
        {
            return string.Format("<{0} field=\"{1}\" />\r\n", compOp.XmlTagName, XMLStringFormatter.EscapeAttributeText(Field.ToString()));
        }

        private string fieldOrStringXml()
        {
            var xmlString = new StringBuilder();

            xmlString.AppendFormat("<{0} field=\"{1}\">\r\n", compOp.XmlTagName, XMLStringFormatter.EscapeAttributeText(Field.ToString()));

            xmlString.Append(expression.ToXml());

            xmlString.AppendFormat("</{0}>\r\n", compOp.XmlTagName);

            return xmlString.ToString();
        }

        private bool conditionComparesMcqToLiteralChoice()
        {
            return (compOp is MCManyOperator || compOp is MCOneOperator) && expression.HasSingleChoiceField;
        }

        [OnSerializing]
        private void onSerializing(StreamingContext context)
        {
            fieldSerializationInfo = new FieldSerializationInfo(Field);
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            Field = fieldSerializationInfo.Deserialized();
            fieldSerializationInfo = null;
        }
    }

    [Serializable]
    public class FibCondition : Condition
    {
        public FibCondition(IXmlElement element, IField fieldResolver) : base(element, fieldResolver)
        {
            compOp = HybridOperator.List[element.Name];
            expression = expressionFactory.MakeObject(element, fieldResolver);
        }

        public FibCondition(IXmlElement element, string processName) : base(element, processName)
        {
            compOp = HybridOperator.List[element.Name];
            expression = expressionFactory.MakeObject(element, processName);
        }

        public FibCondition(IXmlElement element, Process process) : base(element, process)
        {
            compOp = HybridOperator.List[element.Name];
            expression = expressionFactory.MakeObject(element, process);
        }
    }

    [Serializable]
    public class FibNoExpressionCondition : Condition
    {
        public FibNoExpressionCondition(IXmlElement element, IField fieldResolver) : base(element, fieldResolver)
        {
            compOp = HybridOperator.List[element.Name];
            expression = new Expression();
        }

        public FibNoExpressionCondition(IXmlElement element, string processName) : base(element, processName)
        {
            compOp = HybridOperator.List[element.Name];
            expression = new Expression();
        }

        public FibNoExpressionCondition(IXmlElement element, Process process) : base(element, process)
        {
            compOp = HybridOperator.List[element.Name];
            expression = new Expression();
        }
    }

    [Serializable]
    public class MCBaseCondition : Condition
    {
        public MCBaseCondition(IXmlElement element, IField fieldResolver) : base(element, fieldResolver)
        {
            constructComparisonOperator(element);
        }

        public MCBaseCondition(IXmlElement element, string processName) : base(element, processName)
        {
            constructComparisonOperator(element);
        }

        public MCBaseCondition(IXmlElement element, Process process) : base(element, process)
        {
            constructComparisonOperator(element);
        }

        private void constructComparisonOperator(IXmlElement element)
        {
            var mcField = field as IMcqItem;
            if (field is RecordField)
            {
                mcField = ((RecordField)field).ReferenceField as IMcqItem;
            }

            if (mcField != null)
            {
                if (mcField.SelectOnlyOne)
                {
                    compOp = MCOneOperator.List[element.Name] ?? defaultMcOneOperator();
                }
                else
                {
                    compOp = MCManyOperator.List[element.Name] ?? defaultMcManyOperator();
                }
            }
            else
            {
                //Debug.Assert(false, "Expected field to be of type IMcqItem");
                compOp = MCManyOperator.List[element.Name] ?? defaultMcManyOperator();
            }
        }

        private static MCOneOperator defaultMcOneOperator()
        {
            return MCOneOperator.List[0];
        }

        private static MCManyOperator defaultMcManyOperator()
        {
            return MCManyOperator.List[0];
        }
    }

    [Serializable]
    public class MCValueCondition : MCBaseCondition
    {
        public MCValueCondition(IXmlElement element, IField fieldResolver) : base(element, fieldResolver)
        {
            constructExpression(element);
        }

        public MCValueCondition(IXmlElement element, string processName) : base(element, processName)
        {
            constructExpression(element);
        }

        public MCValueCondition(IXmlElement element, Process process) : base(element, process)
        {
            constructExpression(element);
        }

        private void constructExpression(IXmlElement element)
        {
            var choiceField = new ChoiceField(element.GetAttribute("value"));
            expression = new Expression(choiceField);
        }
    }

    [Serializable]
    public class MCFieldCondition : MCBaseCondition
    {
        public MCFieldCondition(IXmlElement element, IField fieldResolver) : base(element, fieldResolver)
        {
            expression = expressionFactory.MakeObject(element.GetChild("string"), fieldResolver);
        }

        public MCFieldCondition(IXmlElement element, string processName) : base(element, processName)
        {
            expression = expressionFactory.MakeObject(element.GetChild("string"), processName);
        }

        public MCFieldCondition(IXmlElement element, Process process) : base(element, process)
        {
            expression = expressionFactory.MakeObject(element.GetChild("string"), process);
        }
    }

    [Serializable]
    public class MCFieldNoExpressionCondition : MCBaseCondition
    {
        public MCFieldNoExpressionCondition(IXmlElement element, IField fieldResolver) : base(element, fieldResolver)
        {
            constructEmptyExpression(element);
        }

        public MCFieldNoExpressionCondition(IXmlElement element, string processName) : base(element, processName)
        {
            constructEmptyExpression(element);
        }

        public MCFieldNoExpressionCondition(IXmlElement element, Process process) : base(element, process)
        {
            constructEmptyExpression(element);
        }

        private void constructEmptyExpression(IXmlElement element)
        {
            expression = new Expression();
        }
    }
}