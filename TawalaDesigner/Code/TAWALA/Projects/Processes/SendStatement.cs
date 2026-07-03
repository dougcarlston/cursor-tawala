// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using Tawala.Common;
using Tawala.Projects.Expressions;
using Tawala.Projects.Factories;
using Tawala.XmlSupport;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements a Show statement in the Process
    /// </summary>
    [Serializable]
    public class SendStatement : ProcessStatement
    {
        private const string xmlSendAddressField = "addressField";
        private const string xmlSendAddressLiteral = "addressLiteral";
        private const string xmlSendCcTag = "<cc {0}=\"{1}\"/>\r\n";
        private const string xmlSendEndTag = "</send>";
        private const string xmlSendStartTag = "<send>\r\n";
        private const string xmlSendSubjectTag = "<subject>{0}</subject>\r\n";
        private const string xmlSendToTag = "<to {0}=\"{1}\"/>\r\n";
        private static readonly Factory<FieldOrLiteral> fieldOrLiteralFactory = new Factory<FieldOrLiteral>();
        private static readonly Factory<SendBody> sendBodyFactory = new Factory<SendBody>();

        /// <summary>
        /// email address(es) of copied recipient(s) ("Cc:")
        /// </summary>
        private FieldOrLiteral addressCc = new FieldOrLiteral();

        private Expression addressFrom = Expression.NULL;

        /// <summary>
        /// email address(es) of main recipient(s) ("To:")
        /// </summary>
        protected FieldOrLiteral addressTo = new FieldOrLiteral();

        private Expression aliasFrom = Expression.NULL;

        /// <summary>
        /// body of the email
        /// </summary>
        protected SendBody sendBody = SendBody.NULL;

        private Expression subjectExpression = Expression.NULL;

        static SendStatement()
        {
            fieldOrLiteralFactory.Register("to", "addressLiteral", typeof(AddressLiteral));
            fieldOrLiteralFactory.Register("cc", "addressLiteral", typeof(AddressLiteral));
            fieldOrLiteralFactory.Register("to", "addressField", typeof(AddressField));
            fieldOrLiteralFactory.Register("cc", "addressField", typeof(AddressField));

            sendBodyFactory.Register("body", typeof(SendForeignInvitationBody), "inviteTo", "project");
            sendBodyFactory.Register("body", typeof(SendInvitationBody), "inviteTo");
            sendBodyFactory.Register("body", typeof(SendDocumentBody), "document");
            sendBodyFactory.Register("body", typeof(SendEmailBody));
        }

        public SendStatement()
        {
            name = "Send";
        }

        /// <summary>
        /// Construct SendStatement from XML "<send>" element.
        /// </summary>
        public SendStatement(IXmlElement element, IField fieldResolver) : this()
        {
            construct(element, fieldResolver);
        }

        /// <summary>
        /// Construct SendStatement from XML "<send>" element.
        /// </summary>
        public SendStatement(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
        {
        }

        public SendStatement(IXmlElement element, Process process) : this()
        {
            construct(element, process.MappedFields.AllFields);
        }

        public FieldOrLiteral AddressTo { get { return addressTo; } set { addressTo = value; } }

        // jdf		//protected Expression addressTo = Expression.Null;

        //public Expression AddressTo
        //{
        //    get { return addressTo; }
        //    set { addressTo = value; }
        //}

        public Expression AddressFrom { get { return addressFrom; } set { addressFrom = value; } }

        public Expression AliasFrom { get { return aliasFrom; } set { aliasFrom = value; } }

        public FieldOrLiteral AddressCc { get { return addressCc; } set { addressCc = value; } }

        public string Subject { get { return subjectExpression.ToString(); } set { subjectExpression = new NonArithmeticExpression(value); } }

        public SendBody SendBody { get { return sendBody; } set { sendBody = value; } }

        /// <summary>
        /// Construct SendStatement from XML <send> element.
        /// </summary>
        private void construct(IXmlElement element, IField fieldResolver)
        {
            addressTo = fieldOrLiteralFactory.MakeObject(element.GetChild("to"), fieldResolver);
            // jdf			//IXmlElement toElement = element.GetChild("to");
            //if (toElement != null)
            //{
            //    if (toElement.HasAttribute("addressField"))
            //    {
            //        this.addressTo = new Expression("<<" + toElement.GetAttribute("addressField") + ">>");
            //    }
            //    else if (toElement.HasAttribute("addressLiteral"))
            //    {
            //        this.addressTo = new Expression(toElement.GetAttribute("addressField"));
            //    }
            //}

            var fromClause = new FromClause(element.GetChild("from"));
            addressFrom = fromClause.Address;
            aliasFrom = fromClause.Alias;

            subjectExpression = new NonArithmeticExpression(element.GetChild("subject"), fieldResolver);
            addressCc = new FieldOrLiteral();

            if (element.GetChild("cc") != XmlElement.NULL)
            {
                addressCc = fieldOrLiteralFactory.MakeObject(element.GetChild("cc"), fieldResolver);
            }

            sendBody = sendBodyFactory.MakeObject(element.GetChild("body"));
        }

        public override string ToString()
        {
            var sendString = new StringBuilder(name + " " + sendBody + " to " + addressTo);

            if (sendBody is SendDocumentBody && ((SendDocumentBody)sendBody).ResetDocumentAfterSend)
            {
                sendString.Append(" and reset Document");
            }

            return sendString.ToString();
        }

        protected string startXml()
        {
            var xmlString = new StringBuilder(xmlSendStartTag);

            string addressType = addressTo.Type == FieldOrLiteral.StringType.field ? xmlSendAddressField : xmlSendAddressLiteral;
            xmlString.AppendFormat(xmlSendToTag, addressType, XMLStringFormatter.EscapeAttributeText(addressTo.Text));

            xmlString.Append(replyInfoXml());
            addressType = addressCc.Type == FieldOrLiteral.StringType.field ? xmlSendAddressField : xmlSendAddressLiteral;

            if (addressCc != null && addressCc.Text.Length > 0)
            {
                addressType = addressCc.Type == FieldOrLiteral.StringType.field ? xmlSendAddressField : xmlSendAddressLiteral;
                xmlString.AppendFormat(xmlSendCcTag, addressType, XMLStringFormatter.EscapeAttributeText(addressCc.Text));
            }

            xmlString.Append(subjectXml());

            return xmlString.ToString();
        }

        private string replyInfoXml()
        {
            var xmlString = new StringBuilder();

            if (replyInfoExists())
            {
                xmlString.Append("<from");
                xmlString.Append(singleElementExpressionXml(addressFrom, "address"));
                xmlString.Append(singleElementExpressionXml(aliasFrom, "alias"));
                xmlString.Append("/>\r\n");
            }

            return xmlString.ToString();
        }

        private bool replyInfoExists()
        {
            return containsSingleElement(addressFrom) || containsSingleElement(aliasFrom);
        }

        private string singleElementExpressionXml(Expression expression, string attributePrefix)
        {
            var xmlString = new StringBuilder();

            if (containsSingleElement(expression))
            {
                if (expression.HasSingleFieldElement)
                {
                    xmlString.AppendFormat(" {0}Field=\"{1}\"", attributePrefix, expression.ToString().Replace("<<", "").Replace(">>", ""));
                }
                else if (expression.HasSingleStringElement)
                {
                    xmlString.AppendFormat(" {0}Literal=\"{1}\"", attributePrefix, expression.ToString().Replace("<<", "").Replace(">>", ""));
                }
            }

            return xmlString.ToString();
        }

        private static bool containsSingleElement(Expression expression)
        {
            return expression.Elements.Count == 1;
        }

        private string subjectXml()
        {
            return string.Format(xmlSendSubjectTag, subjectExpression.ToXml());
        }

        protected string endXml()
        {
            return xmlSendEndTag;
        }

        public override string ToXml()
        {
            var xmlString = new StringBuilder(startXml());
            xmlString.Append(sendBody.ToXml());
            xmlString.Append(endXml());

            return xmlString.ToString();
        }

        #region Nested type: AddressField

        /// <summary>
        /// Specialization of FieldOrLiteral class for use with Factory
        /// </summary>
        [Serializable]
        private class AddressField : FieldOrLiteral
        {
            public AddressField(IXmlElement element, IField fieldResolver)
                : base(element.GetAttribute("addressField"), StringType.field, fieldResolver)
            {
            }
        }

        #endregion

        #region Nested type: AddressFieldExpression

        /// <summary>
        /// Specialization of Expression class for use with Factory
        /// </summary>
        [Serializable]
        private class AddressFieldExpression : Expression
        {
            public AddressFieldExpression(IXmlElement element, IField fieldResolver)
                : base("<<" + element.GetAttribute("addressField") + ">>")
            {
            }
        }

        #endregion

        #region Nested type: AddressLiteral

        /// <summary>
        /// Specialization of FieldOrLiteral class for use with Factory
        /// </summary>
        [Serializable]
        private class AddressLiteral : FieldOrLiteral
        {
            public AddressLiteral(IXmlElement element) : base(element.GetAttribute("addressLiteral"), StringType.literal)
            {
            }

            public AddressLiteral(IXmlElement element, IField fieldResolver) : this(element)
            {
            }
        }

        #endregion

        #region Nested type: AddressLiteralExpression

        /// <summary>
        /// Specialization of Expression class for use with Factory
        /// </summary>
        [Serializable]
        private class AddressLiteralExpression : Expression
        {
            public AddressLiteralExpression(IXmlElement element, IField fieldResolver) : base(element.GetAttribute("addressLiteral"))
            {
            }
        }

        #endregion

        #region Nested type: EmailFieldExpression

        [Serializable]
        private class EmailFieldExpression : Expression
        {
            public EmailFieldExpression(string attributeString) : base("<<" + attributeString + ">>")
            {
            }
        }

        #endregion

        #region Nested type: EmailLiteralExpression

        [Serializable]
        private class EmailLiteralExpression : Expression
        {
            public EmailLiteralExpression(string attributeString) : base(attributeString)
            {
            }
        }

        #endregion

        #region Nested type: FromClause

        [Serializable]
        private class FromClause
        {
            private readonly Expression address = Expression.NULL;
            private readonly Expression alias = Expression.NULL;

            /// <summary>
            /// Constructs a FromClause object from a &lt;from&gt; XML element.
            public FromClause(IXmlElement element)
            {
                if (element.HasAttribute("addressField"))
                {
                    address = new EmailFieldExpression(element.GetAttribute("addressField"));
                }

                if (element.HasAttribute("addressLiteral"))
                {
                    address = new EmailLiteralExpression(element.GetAttribute("addressLiteral"));
                }

                if (element.HasAttribute("aliasField"))
                {
                    alias = new EmailFieldExpression(element.GetAttribute("aliasField"));
                }

                if (element.HasAttribute("aliasLiteral"))
                {
                    alias = new EmailLiteralExpression(element.GetAttribute("aliasLiteral"));
                }
            }

            public Expression Address { get { return address; } }

            public Expression Alias { get { return alias; } }
        }

        #endregion
    }
}