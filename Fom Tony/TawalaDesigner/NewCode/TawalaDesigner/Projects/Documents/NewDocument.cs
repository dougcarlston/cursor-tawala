// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using Tawala.Projects.Factories;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.XmlSupport;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public class NewDocument : IDocument
    {
        private string name = string.Empty;

        public NewDocument(string name)
        {
            Name = name;
            NewContents = FormItemContents.NULL;
        }

        public NewDocument(IXmlElement element)
        {
            Name = element.GetAttribute("name");
            NewContents = FormItemContentsFactory.MakeChildren(element.GetChild("xmlData"));
        }

        #region IDocument Members

        public string Name
        {
            get { return name; }
            set { name = value.Trim(); }
        }

        public override string ToString()
        {
            return Name;
        }

        public string ToXml()
        {
            var sb = new StringBuilder(256);
            sb.AppendFormat("<document name=\"{0}\">\r\n<xmlData>{1}</xmlData>\r\n</document>\r\n", Name, NewContents.ToXml());
            return sb.ToString();
        }

        public string UserVisibleComponentTypeName
        {
            get { throw new NotImplementedException(); }
        }

        public FieldList GetFields()
        {
            return new FieldList();
        }

        public IFormItemContents NewContents { get; set; }

        #endregion
    }
}