// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    /// <summary>
    /// Class to encapsulate Skip Instructions
    /// </summary>
    [Serializable]
    public class SkipInstructions : Process, ISkipInstructions
    {
        private static readonly string xmlSkipInstructionsEndTag = "</skipInstructions>\r\n";
        private static readonly string xmlSkipInstructionsStartTag = "<skipInstructions>\r\n";

        public SkipInstructions() : base(string.Empty)
        {
        }

        public SkipInstructions(IXmlElement element) : base(element)
        {
        }

        public override string Name
        {
            get
            {
                if (base.Name.Length == 0)
                {
                    foreach (IForm form in Project.Current.FormList)
                    {
                        foreach (IFormItem formItem in form.ItemList)
                        {
                            if (formItem is SkipInstructionsItem)
                            {
                                if (((SkipInstructionsItem)formItem).Instructions == this)
                                {
                                    base.Name = form.Name;
                                }
                            }
                        }
                    }
                }
                return base.Name;
            }
            set { base.Name = value; }
        }

        #region ISkipInstructions Members

        public override string ToXml()
        {
            return xmlSkipInstructionsStartTag + Lines.ToXml() + xmlSkipInstructionsEndTag;
        }

        #endregion
    }
}