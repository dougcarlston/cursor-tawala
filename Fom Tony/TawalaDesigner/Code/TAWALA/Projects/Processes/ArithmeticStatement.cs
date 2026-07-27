// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using Tawala.Common;
using Tawala.Projects.Expressions;
using Tawala.Projects.Factories;
using Tawala.Projects.Fields;
using Tawala.XmlSupport;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Base class for Arithmetic Process statements (Add, Subtract, Multiply and Divide).
    /// </summary>
    [Serializable]
    public class ArithmeticStatement : ProcessStatement
    {
        private static readonly string xmlArithmeticEndTag = "</$OPERATOR>";
        private static readonly string xmlArithmeticStartTag = "<$OPERATOR field=\"$VARIABLE\">\r\n";
        private static readonly string xmlOperandFieldTag = "<operand field=\"$FIELD\"/>\r\n";
        private static readonly string xmlOperandValueTag = "<operand value=\"$VALUE\"/>\r\n";
        protected static Factory<FieldOrLiteral> operandFactory = new Factory<FieldOrLiteral>();

        /// <summary>
        /// The Process containing this statement
        /// </summary>
        private readonly Process process;

        protected string operatorString = "unknown operator string";

        /// <summary>
        /// The val to be added to the statement's field component.
        /// </summary>
        protected FieldOrLiteral val = new FieldOrLiteral();

        /// <summary>
        /// The statement's field component (the variable).
        /// </summary>
        protected string variable = "";

        static ArithmeticStatement()
        {
            operandFactory.Register("operand", "value", typeof(ValueOperand));
            operandFactory.Register("operand", "field", typeof(FieldOperand));
        }

        public ArithmeticStatement()
        {
        }

        public ArithmeticStatement(string name, string operatorString)
        {
            this.name = name;
            this.operatorString = operatorString;
        }

        /// <summary>
        /// Construct ArithmeticStatement from XML element
        /// </summary>
        protected ArithmeticStatement(IXmlElement element, string processName, string name, string operatorString)
            : this(element, Project.Current.GetProcess(processName), name, operatorString)
        {
        }

        protected ArithmeticStatement(IXmlElement element, Process process, string name, string operatorString) : this(name, operatorString)
        {
            this.process = process;

            variable = element.GetAttribute("field");
            val = operandFactory.MakeObject(element.GetChild("operand"), process);

            if (isVariable(variable))
            {
                addVariableToProcess();
            }
        }

        public string Variable { get { return variable; } set { variable = value; } }

        public FieldOrLiteral Value { get { return val; } set { val = value; } }

        private void addVariableToProcess()
        {
            if (process != null)
            {
                if (isVariable(variable))
                {
                    process.Variables.AddUnique(variable);
                }
            }
        }

        private bool isVariable(string fieldString)
        {
            return FieldUtil.IsVariable(fieldString);
        }

        public override string ToXml()
        {
            // start with placeholder string
            var xmlString = new StringBuilder(xmlArithmeticStartTag);

            // now insert the field name
            xmlString.Replace("$VARIABLE", XMLStringFormatter.EscapeAttributeText(variable));

            // the <operand> tag depends on the value type
            if (val.Type == FieldOrLiteral.StringType.field)
            {
                // field label
                xmlString.Append(xmlOperandFieldTag);
                xmlString.Replace("$FIELD", XMLStringFormatter.EscapeAttributeText(val.Text));
            }
            else
            {
                // literal string
                xmlString.Append(xmlOperandValueTag);
                xmlString.Replace("$VALUE", XMLStringFormatter.EscapeAttributeText(val.Text));
            }

            // and the end tag for the operator
            xmlString.Append(xmlArithmeticEndTag);

            // and replace the placeholders with the operator text
            xmlString.Replace("$OPERATOR", operatorString);

            return xmlString.ToString();
        }
    }

    /// <summary>
    /// Implements an Add statement
    /// </summary>
    [Serializable]
    public class AddStatement : ArithmeticStatement
    {
        public AddStatement() : base("Add", "addTo")
        {
        }

        /// <summary>
        /// Construct AddStatement from XML element (e.g. <addTo field="Score">)
        /// </summary>
        public AddStatement(IXmlElement element, string processName) : base(element, processName, "Add", "addTo")
        {
        }

        public AddStatement(IXmlElement element, Process process) : base(element, process, "Add", "addTo")
        {
        }

        /// <summary>
        /// Provide statement in plain text form.
        /// </summary>
        public override string ToString()
        {
            return Name + " " + val + " to " + variable;
        }
    }

    /// <summary>
    /// Implements an Subtract statement
    /// </summary>
    [Serializable]
    public class SubtractStatement : ArithmeticStatement
    {
        public SubtractStatement() : base("Subtract", "subtractFrom")
        {
        }

        /// <summary>
        /// Construct SubtractStatement from XML element (e.g. <subtractFrom field="Score">)
        /// </summary>
        public SubtractStatement(IXmlElement element, string processName) : base(element, processName, "Subtract", "subtractFrom")
        {
        }

        public SubtractStatement(IXmlElement element, Process process) : base(element, process, "Subtract", "subtractFrom")
        {
        }

        /// <summary>
        /// Provide statement in plain text form.
        /// </summary>
        public override string ToString()
        {
            return Name + " " + val + " from " + variable;
        }
    }

    /// <summary>
    /// Implements an Multiply statement
    /// </summary>
    [Serializable]
    public class MultiplyStatement : ArithmeticStatement
    {
        public MultiplyStatement() : base("Multiply", "multiplyBy")
        {
        }

        /// <summary>
        /// Construct MultiplyStatement from XML element (e.g. <multiplyBy field="Score">)
        /// </summary>
        public MultiplyStatement(IXmlElement element, string processName) : base(element, processName, "Multiply", "multiplyBy")
        {
        }

        public MultiplyStatement(IXmlElement element, Process process) : base(element, process, "Multiply", "multiplyBy")
        {
        }

        /// <summary>
        /// Provide statement in plain text form.
        /// </summary>
        public override string ToString()
        {
            return Name + " " + variable + " by " + val;
        }
    }

    /// <summary>
    /// Implements an Divide statement
    /// </summary>
    [Serializable]
    public class DivideStatement : ArithmeticStatement
    {
        public DivideStatement() : base("Divide", "divideBy")
        {
        }

        /// <summary>
        /// Construct DivideStatement from XML element (e.g. <multiplyBy field="Score">)
        /// </summary>
        public DivideStatement(IXmlElement element, string processName) : base(element, processName, "Divide", "divideBy")
        {
        }

        public DivideStatement(IXmlElement element, Process process) : base(element, process, "Divide", "divideBy")
        {
        }

        /// <summary>
        /// Provide statement in plain text form.
        /// </summary>
        public override string ToString()
        {
            return Name + " " + variable + " by " + val;
        }
    }

    [Serializable]
    public class ValueOperand : FieldOrLiteral
    {
        /// <summary>
        /// Construct ValueOperand from XML "<operand value=>" element
        /// </summary>
        public ValueOperand(IXmlElement element, string processName) : base(element.GetAttribute("value"), StringType.literal)
        {
        }

        public ValueOperand(IXmlElement element, Process process) : base(element.GetAttribute("value"), StringType.literal)
        {
        }
    }

    [Serializable]
    public class FieldOperand : FieldOrLiteral
    {
        /// <summary>
        /// Construct FieldOperand from XML "<operand field=>" element
        /// </summary>
        public FieldOperand(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
        {
        }

        public FieldOperand(IXmlElement element, Process process)
        {
            Type = StringType.field;
            RawText = element.GetAttribute("field");
            Expression = new Expression("<<" + RawText + ">>", process.MappedFields[RawText]);
        }
    }
}