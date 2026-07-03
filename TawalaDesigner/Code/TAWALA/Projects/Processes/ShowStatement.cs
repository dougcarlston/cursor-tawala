// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using Tawala.Common;
using Tawala.Projects.Components;
using Tawala.Projects.Documents;
using Tawala.XmlSupport;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements a Show statement in the Process.  
    /// Note that it requires deserialization fixups under some circumstances.  See onDeserialized method.
    /// </summary>
    [Serializable]
    public class ShowStatement : ProcessStatement
    {
        private const string xmlShowTags = "<show {0}=\"{1}\"/>";

        [NonSerialized]
        protected IProjectComponent component;

        protected string serializedComponentName;

        public ShowStatement()
        {
            name = "Show";
            // This Assert is here because ShowStatement should essentially be abstract
            // But the Details base class in Processes project expects this to have a public constructor.
            Debug.Assert(GetType().Name.CompareTo(typeof(ShowStatement).Name) != 0);
        }

        public IDocument Document { get { return component as IDocument; } set { component = value; } }

        public IForm Form { get { return component as IForm; } set { component = value; } }

        public override string ToString()
        {
            // once we are sure this is obsolete, let's get rid of it.
            Debug.Assert(false, "ShowStatement.ToString() shouldn't be called.");
            return (Name + " " + (component is Document ? "Document" : "Form") + " " + component.Name);
        }

        public override string ToXml()
        {
            // once we are sure this is obsolete, let's get rid of it.
            Debug.Assert(false, "ShowStatement.ToXml() shouldn't be called.");
            return
                new StringBuilder().AppendFormat(xmlShowTags, (component is Document ? "document" : "form"),
                                                 XMLStringFormatter.EscapeAttributeText(component.Name)).ToString();
        }

        [OnSerializing]
        private void onSerializing(StreamingContext context)
        {
            //            Debug.Assert(string.IsNullOrEmpty(serializedComponentName));
            serializedComponentName = component != null ? component.Name : null;
        }
    }

    [Serializable]
    public class ShowDocumentStatement : ShowStatement
    {
        private const string xmlShowDocumentString = "<show document=\"{0}\" reset=\"{1}\"/>";
        private bool resetAfterShow;

        public ShowDocumentStatement()
        {
        }

        public ShowDocumentStatement(IDocument document) : this()
        {
            component = document;
        }

        public ShowDocumentStatement(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
        {
        }

        public ShowDocumentStatement(IXmlElement element, Process process) : this()
        {
            component = Project.Current.GetRealOrVirtualDocument(element.GetAttribute("document"), false);
            if (component == null)
            {
                component = Documents.Document.NULL;
            }
            resetAfterShow = element.GetAttribute("reset") == "true" ? true : false;
        }

        public bool ResetAfterShow { get { return resetAfterShow; } set { resetAfterShow = value; } }

        public override Type GetStatementType()
        {
            return typeof(ShowStatement);
        }

        public override string ToString()
        {
            return (Name + (resetAfterShow ? " and reset" : "") + " Document " + component.Name);
        }

        //public override string ToXml()
        //{
        //    StringBuilder xmlString =  new StringBuilder();
        //    xmlString.AppendFormat(xmlShowDocumentString, XMLStringFormatter.EscapeAttributeText(component.Name), resetAfterShow ? "true" : "false");
        //    return xmlString.ToString();
        //}

        public override string ToXml()
        {
            string documentName = component.Name;

            if (!Project.Current.DocumentList.Contains(Document))
            {
                documentName = Documents.Document.NULL.Name;
            }

            var xmlString = new StringBuilder();
            xmlString.AppendFormat(xmlShowDocumentString, XMLStringFormatter.EscapeAttributeText(documentName),
                                   resetAfterShow ? "true" : "false");
            return xmlString.ToString();
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            Debug.Assert(!string.IsNullOrEmpty(serializedComponentName));

            component = Project.Current.GetRealOrVirtualDocument(serializedComponentName, false);
            if (component == null)
            {
                if (serializedComponentName.Equals(Documents.Document.NULL.Name))
                {
                    component = Documents.Document.NULL;
                }
                else
                {
                    component = new RtfDocument(serializedComponentName);
                    Project.Current.AddDocument(component as RtfDocument);
                }
            }

            serializedComponentName = null;
        }
    }

    [Serializable]
    public class ShowFormStatement : ShowStatement
    {
        private const string xmlShowFormString = "<show form=\"{0}\"/>";

        public ShowFormStatement()
        {
        }

        public ShowFormStatement(IForm form) : this()
        {
            component = form;
        }

        public ShowFormStatement(IXmlElement element, string processName) : this(element, Project.Current.GetProcess(processName))
        {
        }

        public ShowFormStatement(IXmlElement element, Process process) : this()
        {
            component = Project.Current.GetForm(element.GetAttribute("form"));
            if (component == null)
            {
                component = Projects.Form.NULL;
            }
        }

        public override Type GetStatementType()
        {
            return typeof(ShowStatement);
        }

        public override string ToString()
        {
            return (Name + " Form " + component.Name);
        }

        //public override string ToXml()
        //{
        //    return new StringBuilder().AppendFormat(xmlShowFormString, XMLStringFormatter.EscapeAttributeText(component.Name)).ToString();
        //}

        public override string ToXml()
        {
            string formName = component.Name;

            if (!Project.Current.FormList.Contains(Form))
            {
                formName = Projects.Form.NULL.Name;
            }

            return new StringBuilder().AppendFormat(xmlShowFormString, XMLStringFormatter.EscapeAttributeText(formName)).ToString();
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            Debug.Assert(!string.IsNullOrEmpty(serializedComponentName));

            foreach (IForm form in Project.Current.FormList)
            {
                if (form.Name.CompareTo(serializedComponentName) == 0)
                {
                    component = form;
                    break;
                }
            }

            component = Project.Current.GetForm(serializedComponentName);

            if (component == null)
            {
                IForm form = Project.Current.AddForm();
                form.Name = serializedComponentName;
                component = form;
            }
            serializedComponentName = null;
        }
    }
}