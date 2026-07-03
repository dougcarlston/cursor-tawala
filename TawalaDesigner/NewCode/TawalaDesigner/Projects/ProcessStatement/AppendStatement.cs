// $Workfile: AppendStatement.cs $
// $Revision: 17 $	$Date: 12/17/07 4:48p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using Tawala.Common;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements a Append statement in the Process.  
	///		APPEND appendage TO document
	/// </summary>
	[Serializable]
	public class AppendStatement : ProcessStatement
	{
		public AppendStatement()
		{
			name = "Append";
		}

		public AppendStatement(IDocument appendage, IDocument document) : this()
		{
			this.appendage = appendage;
			this.document = document;
		}

		public AppendStatement(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
		{
		}

		public AppendStatement(IXmlElement element, Process process) : this()
		{
			this.appendage = Project.Current.GetDocument(element.GetAttribute("appendage"));
			this.document = Project.Current.GetRealOrVirtualDocument(element.GetAttribute("document"), true);
		}

		/// <summary>
		/// Document that will be appended at end of target.
		/// </summary>
		[NonSerialized]
        protected IDocument appendage = NullObjects.Document;

		public IDocument Appendage
		{
			get
			{
				return appendage;
			}
			set
			{
				appendage = value;
			}
		}

		/// <summary>
		/// Document which will be appended to
		/// </summary>
        [NonSerialized]
        protected IDocument document = NullObjects.Document;

		public IDocument Document
		{
			get
			{
				return document;
			}
			set
			{
				document = value;
			}
		}

		/// <summary>
		/// Provide statement in plain text form.
		/// </summary>
		public override string ToString()
		{
			return (Name + " " + appendage.Name + " to " + document.Name);
		}

		private const string xmlAppendDocumentTags = "<append document=\"$DOCUMENT\" appendage=\"$APPENDAGE\"/>";

		public override string ToXml()
		{
			// start with placeholder string
			StringBuilder xmlString = new StringBuilder(xmlAppendDocumentTags);

			xmlString.Replace("$DOCUMENT", XMLStringFormatter.EscapeAttributeText(document.Name));
			xmlString.Replace("$APPENDAGE", XMLStringFormatter.EscapeAttributeText(appendage.Name));

			return xmlString.ToString();
		}

		public override IProcessElement AsProcessElement()
		{
			return new AppendLine(this);
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
            private string documentName;
            private bool isNullDocument;
            private string appendageName;
            private bool isNullAppendage;

            public SerializationInfo(AppendStatement a)
            {
                documentName = a.document.Name;
                appendageName = a.appendage.Name;
                isNullDocument = a.document == NullObjects.Document || a.document == null;
                isNullAppendage = a.appendage == NullObjects.Document || a.appendage == null;
            }

            public void Deserialized(AppendStatement a)
            {
                if (isNullAppendage)
                {
                    a.appendage = NullObjects.Document;
                }
                else
                {
                    a.appendage = Project.Current.GetRealOrVirtualDocument(appendageName, false);
                }

                if (isNullDocument)
                {
                    a.document = NullObjects.Document;
                }
                else
                {
                    a.document = Project.Current.GetRealOrVirtualDocument(documentName, false);
                }

				if (a.appendage == null)
				{
                    a.appendage = NullObjects.Document;
				}

				if (a.document == null)
				{
                    a.document = NullObjects.Document;
				}
            }


        }
	}
}
