// Copyright © 2005 - 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Diagnostics;
using System.Text;
using Tawala.Common;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements a Show statement in the Process
	/// </summary>
	[Serializable]
	public class SendStatement : ProcessStatement
	{
		private static Factory<FieldOrLiteral> fieldOrLiteralFactory = new Factory<FieldOrLiteral>();
		private static Factory<SendBody> sendBodyFactory = new Factory<SendBody>();

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

		[Serializable]
		private class FromClause
		{
			private Expression address = Expression.NULL;
			private Expression alias = Expression.NULL;

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

			public Expression Address
			{
				get { return address; }
			}

			public Expression Alias
			{
				get { return alias; }
			}
		}

		[Serializable]
		private class EmailFieldExpression : Expression
		{
			public EmailFieldExpression(string attributeString) : base("<<" + attributeString + ">>")
			{
			}
		}

		[Serializable]
		private class EmailLiteralExpression : Expression
		{
			public EmailLiteralExpression(string attributeString) : base(attributeString)
			{
			}
		}

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

		/// <summary>
		/// Specialization of FieldOrLiteral class for use with Factory
		/// </summary>
		[Serializable]
		private class AddressField : FieldOrLiteral
		{
			public AddressField(IXmlElement element, IField fieldResolver) : base(element.GetAttribute("addressField"), StringType.field, fieldResolver)
			{
			}
		}

		/// <summary>
		/// Specialization of Expression class for use with Factory
		/// </summary>
		[Serializable]
		private class AddressFieldExpression : Expression
		{
			public AddressFieldExpression(IXmlElement element, IField fieldResolver) : base("<<" + element.GetAttribute("addressField") + ">>")
			{
			}
		}

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

		/// <summary>
		/// Construct SendStatement from XML <send> element.
		/// </summary>
		private void construct(IXmlElement element, IField fieldResolver)
		{
			this.addressTo = fieldOrLiteralFactory.MakeObject(element.GetChild("to"), fieldResolver);
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
			
			FromClause fromClause = new FromClause(element.GetChild("from"));
			this.addressFrom = fromClause.Address;
			this.aliasFrom = fromClause.Alias;
			
			this.subjectExpression = new NonArithmeticExpression(element.GetChild("subject"), fieldResolver);
			this.addressCc = new FieldOrLiteral();

			if (element.GetChild("cc") != XmlElement.NULL)
			{
				this.addressCc = fieldOrLiteralFactory.MakeObject(element.GetChild("cc"), fieldResolver);
			}

			this.sendBody = sendBodyFactory.MakeObject(element.GetChild("body"));
		}

		/// <summary>
		/// email address(es) of main recipient(s) ("To:")
		/// </summary>
		protected FieldOrLiteral addressTo = new FieldOrLiteral();

		public FieldOrLiteral AddressTo
		{
			get { return addressTo; }
			set { addressTo = value; }
		}
// jdf		//protected Expression addressTo = Expression.NULL;

		//public Expression AddressTo
		//{
		//    get { return addressTo; }
		//    set { addressTo = value; }
		//}

		private Expression addressFrom = Expression.NULL;

		public Expression AddressFrom
		{
			get { return addressFrom; }
			set { addressFrom = value; }
		}

		private Expression aliasFrom = Expression.NULL;

		public Expression AliasFrom
		{
			get { return aliasFrom; }
			set { aliasFrom = value; }
		}

		/// <summary>
		/// email address(es) of copied recipient(s) ("Cc:")
		/// </summary>
		private FieldOrLiteral addressCc = new FieldOrLiteral();

		public FieldOrLiteral AddressCc
		{
			get { return addressCc; }
			set { addressCc = value; }
		}

		private Expression subjectExpression = Expression.NULL;

		public string Subject
		{
			get { return subjectExpression.ToString(); }
			set { subjectExpression = new NonArithmeticExpression(value); }
		}

		/// <summary>
		/// body of the email
		/// </summary>
		protected SendBody sendBody = SendBody.NULL;

		public SendBody SendBody
		{
			get { return sendBody; }
			set { sendBody = value; }
		}

		public override string ToString()
		{
			StringBuilder sendString = new StringBuilder(name + " " + sendBody.ToString() + " to " + addressTo.ToString());

			if (sendBody is SendDocumentBody && ((SendDocumentBody)sendBody).ResetDocumentAfterSend)
			{
				sendString.Append(" and reset Document");
			}

			return sendString.ToString();
		}

		private const string xmlSendStartTag = "<send>\r\n";
		private const string xmlSendEndTag = "</send>";
		private const string xmlSendToTag = "<to {0}=\"{1}\"/>\r\n";
		private const string xmlSendCcTag = "<cc {0}=\"{1}\"/>\r\n";
		private const string xmlSendAddressField = "addressField";
		private const string xmlSendAddressLiteral = "addressLiteral";
		private const string xmlSendSubjectTag = "<subject>{0}</subject>\r\n";

		protected string startXml()
		{
			StringBuilder xmlString = new StringBuilder(xmlSendStartTag);

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
			StringBuilder xmlString = new StringBuilder();

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
			StringBuilder xmlString = new StringBuilder();

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
			StringBuilder xmlString = new StringBuilder(startXml());
			xmlString.Append(sendBody.ToXml());
			xmlString.Append(endXml());

			return xmlString.ToString();
		}
	}

}
