// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Runtime.Serialization;
using System.Text;
using Tawala.Common;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements a Append statement in the Process.  
    ///		APPEND appendage TO document
    /// </summary>
    [Serializable]
    public class AppendStatement : ProcessStatement
    {
        private const string xmlAppendDocumentTags = "<append document=\"$DOCUMENT\" appendage=\"$APPENDAGE\"/>";

        /// <summary>
        /// Document that will be appended at end of target.
        /// </summary>
        [NonSerialized]
        protected IDocument appendage = Documents.Document.NULL;

        /// <summary>
        /// Document which will be appended to
        /// </summary>
        [NonSerialized]
        protected IDocument document = Documents.Document.NULL;

        private SerializationInfo serializationInfo;

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
            appendage = Project.Current.GetDocument(element.GetAttribute("appendage"));
            document = Project.Current.GetRealOrVirtualDocument(element.GetAttribute("document"), true);
        }

        public IDocument Appendage { get { return appendage; } set { appendage = value; } }

        public IDocument Document { get { return document; } set { document = value; } }

        /// <summary>
        /// Provide statement in plain text form.
        /// </summary>
        public override string ToString()
        {
            return (Name + " " + appendage.Name + " to " + document.Name);
        }

        public override string ToXml()
        {
            // start with placeholder string
            var xmlString = new StringBuilder(xmlAppendDocumentTags);

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

        #region Nested type: SerializationInfo

        [Serializable]
        private class SerializationInfo
        {
            private readonly string appendageName;
            private readonly string documentName;
            private readonly bool isNullAppendage;
            private readonly bool isNullDocument;

            public SerializationInfo(AppendStatement a)
            {
                documentName = a.document.Name;
                appendageName = a.appendage.Name;
                isNullDocument = a.document == Documents.Document.NULL || a.document == null;
                isNullAppendage = a.appendage == Documents.Document.NULL || a.appendage == null;
            }

            public void Deserialized(AppendStatement a)
            {
                if (isNullAppendage)
                {
                    a.appendage = Documents.Document.NULL;
                }
                else
                {
                    a.appendage = Project.Current.GetRealOrVirtualDocument(appendageName, false);
                }

                if (isNullDocument)
                {
                    a.document = Documents.Document.NULL;
                }
                else
                {
                    a.document = Project.Current.GetRealOrVirtualDocument(documentName, false);
                }

                if (a.appendage == null)
                {
                    a.appendage = Documents.Document.NULL;
                }

                if (a.document == null)
                {
                    a.document = Documents.Document.NULL;
                }
            }
        }

        #endregion
    }
}