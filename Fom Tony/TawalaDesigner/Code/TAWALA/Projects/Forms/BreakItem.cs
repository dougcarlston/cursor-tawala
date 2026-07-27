// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms
{
    /// <summary>
    /// Class to encapsulate Page Break items on Forms
    /// </summary>
    [Serializable]
    public class BreakItem : FormItem, IField
    {
        private const string xmlBreakItemTag = "<break/>\r\n";

        public BreakItem()
        {
        }

        public BreakItem(IXmlElement element)
        {
        }

        public override string AlternateLabel { get { return string.Empty; } set { } }

        #region IField Interface

        public override string FieldName { get { return "Break Item"; } }

        public override string FieldString { get { return "<<" + FieldName + ">>"; } }

        public override IField this[string name]
        {
            get
            {
                if (FieldName == name)
                {
                    return this;
                }

                return null;
            }
        }

        #endregion

        #region IEnumerable Interface

        public override IEnumerator GetEnumerator()
        {
            yield break;
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public override IEnumerable RecursiveEnumerator { get { yield break; } }

        #endregion

        public override string ToXml()
        {
            return xmlBreakItemTag;
        }
    }
}