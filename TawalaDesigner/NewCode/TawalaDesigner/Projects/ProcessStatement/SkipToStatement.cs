// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using System.Runtime.Serialization;
using Tawala.Common;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements a Skip statement in the Process.  
	/// Note that it requires deserialization fixups under some circumstances.  See onDeserialized method.
	/// </summary>
	[Serializable]
	public class SkipToStatement : ProcessStatement
	{
		public SkipToStatement()
		{
			name = "Skip";
		}

		public SkipToStatement(SkipToDestinationItem destination) : this()
		{
			this.destination = destination;
		}

		public SkipToStatement(IXmlElement element, Process process) : this()
		{
			if (element.GetAttribute("to") == "__EndOfForm__")
			{
				this.destination = new SkipToDestinationItem();
			}
			else
			{
				string unqualifiedFieldName = element.GetAttribute("to");

				IFormItem formItem = (process.MappedFields.ContainsKey(unqualifiedFieldName) ? (IFormItem)process.MappedFields[unqualifiedFieldName] : null);

				if (formItem != null)
				{
					this.destination = new SkipToDestinationItem(formItem);
				}
				else
				{
					this.destination = new SkipToDestinationItem(element.GetAttribute("to"));
				}
			}
		}

		/// <summary>
		/// Place in the Form to "skip to"
		/// </summary>
		/// <remarks>
		/// Will be either a Question Label or a Text Label
		/// </remarks>
		public SkipToDestinationItem Destination
		{
			get
			{
				return destination;
			}
			set
			{
				destination = value;
			}
		}

		private SkipToDestinationItem destination;

		/// <summary>
		/// Provide statement in plain text form.
		/// </summary>
		public override string ToString()
		{
			string label = "";
			if (destination != null)
			{
				label = destination.ToString();
			}
			return (Name + " to " + label);
		}

		/// <summary>
		/// Provide statement in XML form.
		/// </summary>
		private static readonly string xmlShowDocumentTags = "<skip to=\"$DESTINATION\"/>";

		public override string ToXml()
		{
			// start with placeholder string
			StringBuilder xmlString = new StringBuilder(xmlShowDocumentTags);

			// replace text placeholder with actual value
			string label = "";
			if (destination != null)
			{
				label = destination.AttributeString();
			}
			xmlString.Replace("$DESTINATION", XMLStringFormatter.EscapeAttributeText(label));

			return xmlString.ToString();
		}
	}
}
