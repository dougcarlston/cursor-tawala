// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;

namespace Tawala.Projects.Documents
{
    [Serializable]
    public class ParagraphComponent : IParagraphComponent
    {
        public static ParagraphComponent NULL = new ParagraphComponent();

        #region IParagraphComponent Members

        public virtual string Text { get { return ""; } }

        public virtual string ToXml()
        {
            return "";
        }

        public virtual string ToHtml()
        {
            return "";
        }

        public virtual string ToRtf()
        {
            return "";
        }

        #endregion

        public virtual string ToRtf(RtfDocument document)
        {
            return "";
        }

        #region IEnumerable Interface

        public virtual IEnumerator GetEnumerator()
        {
            yield return this;
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public virtual IEnumerable RecursiveEnumerator { get { yield return this; } }

        #endregion
    }
}