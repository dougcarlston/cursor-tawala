// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Parsing;
using Tawala.Projects.Factories;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace Tawala.Projects.Expressions
{
    /// <summary>
    /// Implements expressions used in SET statements
    /// </summary>
    [Serializable]
    public class Expression
    {
        protected const string xmlFieldNameTag = "<field name=\"$FIELD\"/>";
        protected const string xmlStringFieldTag = "<string field=\"$FIELD\"/>\r\n";
        protected const string xmlStringValueTag = "<string value=\"$VALUE\"/>\r\n";
        protected static string fieldPattern;
        public static Expression NULL = new NullExpression();
        protected static Hashtable operatorTable = new Hashtable();
        protected static string stringFieldPattern;
        protected static string stringPattern;

        /// <summary>
        /// List of expression elements
        /// </summary>
        private ExpressionElementList elements;

        static Expression()
        {
            // matches fields (such as "<<Q1:a>>" or "<<Name>>")
            fieldPattern = @"<<((?:(?!>>).)*)>>";

            // matches strings (such as "2" or " - 5")
            stringPattern = @"((?:(?!<<).)+)";

            // combine field and string patterns into one alternation
            stringFieldPattern = fieldPattern + "|" + stringPattern;

            // initialize table of operators
            operatorTable.Add("add", " + ");
            operatorTable.Add("sub", " - ");
            operatorTable.Add("mul", " * ");
            operatorTable.Add("div", " / ");
        }

        public Expression()
        {
        }

        public Expression(string expressionString)
        {
            construct(expressionString, getCurrentFields());
        }

        public Expression(string expressionString, IField fieldResolver)
        {
            construct(expressionString, fieldResolver);
        }

        public Expression(string expressionString, string processName) : this(expressionString, Project.Current.GetProcess(processName))
        {
        }

        public Expression(string expressionString, Process process)
        {
            construct(expressionString, process.MappedFields.AllFields);
        }

        public Expression(Field field)
        {
            elements = new ExpressionElementList();
            elements.Add(new FieldElement(field));
        }

        public Expression(IField field)
        {
            elements = new ExpressionElementList();
            elements.Add(new FieldElement(field));
        }

        public ExpressionElementList Elements { get { return elements; } }

        /// <summary>
        /// Indicates whether this expression has one or more field elements
        /// </summary>
        public Boolean HasFieldElement
        {
            get
            {
                if (elements != null)
                {
                    foreach (ExpressionElement e in elements)
                    {
                        if (e is FieldElement)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Indicates whether this expression has one or more string elements
        /// </summary>
        public Boolean HasStringElement
        {
            get
            {
                if (elements != null)
                {
                    foreach (ExpressionElement e in elements)
                    {
                        if (e is StringElement)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Indicates whether this expression has one and only one field element
        /// </summary>
        public Boolean HasSingleFieldElement { get { return (HasFieldElement && (elements.Count == 1)); } }

        /// <summary>
        /// Indicates whether this expression has one and only one field element representing a choice in a multiple-choice queation
        /// </summary>
        public Boolean HasSingleChoiceField { get { return (HasSingleFieldElement && ((FieldElement)elements[0]).Field is ChoiceField); } }

        /// <summary>
        /// Indicates whether this expression has one and only one string element
        /// </summary>
        public Boolean HasSingleStringElement { get { return (HasStringElement && (elements.Count == 1)); } }

        /// <summary>
        /// Indicates whether this expression has zero elements
        /// </summary>
        public Boolean IsEmpty { get { return (elements.Count == 0); } }

        protected void construct(string expressionString, IField fieldResolver)
        {
            elements = new ExpressionElementList();

            // get collection of field element strings matching
            // either string or field pattern
            MatchCollection matches = Regex.Matches(expressionString, stringFieldPattern);

            // for each match found...
            foreach (Match m in matches)
            {
                // if it is a field pattern...
                if (Regex.IsMatch(m.Value, fieldPattern))
                {
                    IField field;
                    string fieldName = m.Groups[1].Value;

                    if (isVariable(fieldName))
                    {
                        field = new Variable(fieldName);
                    }
                    else
                    {
                        field = fieldResolver[fieldName];
                    }

                    if (field != null)
                    {
                        elements.Add(new FieldElement(field));
                    }
                    else
                    {
                        string qualifiedFieldName = m.Value.Replace("<<", "").Replace(">>", "");

                        if (Project.FieldMapByName[qualifiedFieldName] is IHiddenField)
                        {
                            elements.Add(new FieldElement(Project.FieldMapByName[qualifiedFieldName]));
                        }
                        else if (Project.FieldMapByName[qualifiedFieldName] is IFileUploadItem)
                        {
                            elements.Add(new FieldElement(Project.FieldMapByName[qualifiedFieldName]));
                        }
                        else if (FieldUtil.IsUnknownField(m.Groups[1].Value))
                        {
                            elements.Add(new FieldElement(new Variable(FieldUtil.UnknownFieldName)));
                        }
                            //else if (FieldUtil.IsRecordField(qualifiedFieldName))
                            //{
                            //    Record record = new Record(FieldUtil.GetRecordName(qualifiedFieldName));
                            //    Form form = Project.Current.GetForm(FieldUtil.GetFormName(qualifiedFieldName));
                            //    ExtraField extraField = new ExtraField(FieldUtil.GetFieldName(qualifiedFieldName), form);
                            //    FieldElement fieldElement = new FieldElement(new RecordField(record, extraField));

                            //    elements.Add(fieldElement);
                            //}
                        else
                        {
                            elements.Add(new FieldElement(new UnresolvedField(qualifiedFieldName)));
                        }
                    }
                }
                    // if it is a string pattern...
                else if (Regex.IsMatch(m.Value, stringPattern))
                {
                    // add second captured group as string element to list of elements
                    elements.Add(new StringElement(m.Groups[2].Value));
                }
            }
        }

        private bool isVariable(string fieldName)
        {
            return FieldUtil.IsVariable(fieldName);
        }

        private FieldList getCurrentFields()
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

            if (elements != null)
            {
                foreach (ExpressionElement e in elements)
                {
                    sb.Append(e.ToString());
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// checks for any arithmetic operators in the expressionString
        /// </summary>
        private bool isArithmetic()
        {
            if (ToString().Contains("+") || ToString().Contains("-") || ToString().Contains("*") || ToString().Contains("/"))
            {
                return true;
            }

            return false;
        }

        public virtual string ToXml()
        {
            // start with empty string
            var xmlString = new StringBuilder();

            // add the expression
            if (ToString().Length > 0)
            {
                // look for any arithmetic operators
                if (isArithmetic())
                {
                    // try parsing as an arithmetic expression
                    var parser = new MathParser(ToString());
                    string parsed = parser.ToXml();

                    // if it just don't look good
                    if (parser.Balance != 0 || parsed.Contains("invalidExpression"))
                    {
                        // we'll write it out as a string (removing any special characters)
                        xmlString.Append(xmlStringValueTag);
                        xmlString.Replace("$VALUE", XMLStringFormatter.EscapeAttributeText(ToString()));
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
                    MatchCollection mc = Regex.Matches(ToString(), stringFieldPattern);

                    for (int i = 0; i < mc.Count; i++)
                    {
                        string token = mc[i].ToString();
                        // if it is a field pattern...
                        if (Regex.Match(token, fieldPattern).Success)
                        {
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

            return xmlString.ToString();
        }

        #region Nested type: NullExpression

        [Serializable]
        private class NullExpression : Expression
        {
            public NullExpression()
            {
                elements = new ExpressionElementList();
            }
        }

        #endregion
    }

    [Serializable]
    public class ValueExpression : Expression
    {
        /// <summary>
        /// Construct Expression from XML "<string value=>" element
        /// </summary>
        public ValueExpression(IXmlElement element) : base(element.GetAttribute("value"))
        {
        }

        public ValueExpression(IXmlElement element, string processName) : base(element.GetAttribute("value"))
        {
        }

        public ValueExpression(IXmlElement element, Process process) : base(element.GetAttribute("value"))
        {
        }

        public ValueExpression(IXmlElement element, IField fieldResolver) : this(element)
        {
        }
    }

    [Serializable]
    public class TextExpression : Expression
    {
        /// <summary>
        /// Construct Expression from XML &lt;#text&gt; element
        /// </summary>
        public TextExpression(IXmlElement element, IField fieldResolver) : base(XmlUtility.ConvertEntitiesToText(element.OuterXml))
        {
        }
    }

    [Serializable]
    public class FieldExpression : Expression
    {
        /// <summary>
        /// Construct Expression from XML "<string field=>" element
        /// </summary>
        public FieldExpression(IXmlElement element, IField fieldResolver) : base("<<" + element.GetAttribute("field") + ">>", fieldResolver)
        {
        }

        public FieldExpression(IXmlElement element, string processName) : base("<<" + element.GetAttribute("field") + ">>", processName)
        {
        }

        public FieldExpression(IXmlElement element, Process process) : base("<<" + element.GetAttribute("field") + ">>", process)
        {
        }
    }

    [Serializable]
    public class RationalFieldExpression : Expression
    {
        /// <summary>
        /// Construct Expression from XML "<field name=>" element
        /// </summary>
        public RationalFieldExpression(IXmlElement element) : base("<<" + element.GetAttribute("name") + ">>")
        {
        }

        public RationalFieldExpression(IXmlElement element, IField fieldResolver)
            : base("<<" + element.GetAttribute("name") + ">>", fieldResolver)
        {
        }
    }

    [Serializable]
    public class ArithmeticExpression : Expression
    {
        /// <summary>
        /// Construct Expression from XML arithmetic element (e.g. "<add>", "<sub>", "<mul>", "<div>")
        /// </summary>
        public ArithmeticExpression(IXmlElement element, IField fieldResolver)
        {
            construct(trimOuterParentheses(getExpressionString(element)), fieldResolver);
        }

        public ArithmeticExpression(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
        {
        }

        public ArithmeticExpression(IXmlElement element, Process process)
        {
            construct(trimOuterParentheses(getExpressionString(element)), process.MappedFields.AllFields);
        }

        /// <summary>
        /// This recursive method returns a string containing a parenthesized arithmetic expression. Due to the
        /// simplistic nature of the algorithm, an extra pair of outer parentheses is generated. Use the
        /// trimOuterParentheses method to strip these off.
        /// </summary>
        private string getExpressionString(IXmlElement element)
        {
            string expressionString = "";

            switch (element.Name)
            {
                case "add":
                case "sub":
                case "mul":
                case "div":
                    expressionString += "(" + getExpressionString(element.GetChild(0));
                    expressionString += operatorTable[element.Name];
                    expressionString += getExpressionString(element.GetChild(1)) + ")";
                    break;

                case "operand":
                    return (element.HasAttribute("field") ? ("<<" + element.GetAttribute("field") + ">>") : element.GetAttribute("value"));
            }

            return expressionString;
        }

        private string trimOuterParentheses(string expressionString)
        {
            return (Regex.Replace(expressionString, @"^\((.*)\)$", "$1"));
        }
    }

    [Serializable]
    public class CompoundExpression : Expression
    {
        protected static Factory<Expression> expressionFactory = new Factory<Expression>();

        static CompoundExpression()
        {
            expressionFactory.Register("string", "value", typeof(ValueExpression));
            expressionFactory.Register("operand", "value", typeof(ValueExpression));
            expressionFactory.Register("string", "field", typeof(FieldExpression));
            expressionFactory.Register("field", "name", typeof(RationalFieldExpression));
            expressionFactory.Register("#text", typeof(TextExpression));
            expressionFactory.Register("#whitespace", typeof(TextExpression));

            expressionFactory.Register("add", typeof(ArithmeticExpression));
            expressionFactory.Register("sub", typeof(ArithmeticExpression));
            expressionFactory.Register("mul", typeof(ArithmeticExpression));
            expressionFactory.Register("div", typeof(ArithmeticExpression));
        }

        public CompoundExpression(string expressionString) : base(expressionString)
        {
        }

        /// <summary>
        /// Construct Expression from children of XML element (such as "<set>" or "<equals>")
        /// </summary>
        public CompoundExpression(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
        {
        }

        public CompoundExpression(IXmlElement element, Process process)
        {
            var sb = new StringBuilder();

            int i = 0;

            while (element.GetChild(i) != XmlElement.NULL)
            {
                Expression expression = expressionFactory.MakeObject(element.GetChild(i++), process);
                sb.Append(expression.ToString());
            }

            string expressionString = sb.ToString();
            construct(expressionString, process.MappedFields.AllFields);
        }

        public CompoundExpression(IXmlElement element, IField fieldResolver)
        {
            var sb = new StringBuilder();

            int i = 0;

            while (element.GetChild(i) != XmlElement.NULL)
            {
                Expression expression = expressionFactory.MakeObject(element.GetChild(i++), fieldResolver);
                sb.Append(expression.ToString());
            }

            string expressionString = sb.ToString();
            construct(expressionString, fieldResolver);
        }
    }

    [Serializable]
    public class NonArithmeticExpression : CompoundExpression
    {
        public NonArithmeticExpression(string expressionString) : base(expressionString)
        {
        }

        public NonArithmeticExpression(IXmlElement element, IField fieldResolver) : base(element, fieldResolver)
        {
        }

        public override string ToXml()
        {
            // start with empty string
            var xmlString = new StringBuilder();

            // add the expression
            if (ToString().Length > 0)
            {
                MatchCollection mc = Regex.Matches(ToString(), stringFieldPattern);

                for (int i = 0; i < mc.Count; i++)
                {
                    string token = mc[i].ToString();

                    if (Regex.Match(token, fieldPattern).Success)
                    {
                        xmlString.Append(xmlFieldNameTag);
                        xmlString.Replace("$FIELD", XMLStringFormatter.EscapeAttributeText(token.Substring(2, token.Length - 4)));
                    }
                    else
                    {
                        xmlString.Append(XMLStringFormatter.EscapeElementText(token));
                    }
                }
            }

            return xmlString.ToString();
        }
    }
}