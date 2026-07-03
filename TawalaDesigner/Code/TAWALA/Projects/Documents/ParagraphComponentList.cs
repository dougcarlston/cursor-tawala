// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;

namespace Tawala.Projects.Documents
{
    /// <summary>
    /// List of fields.
    /// </summary>
    [Serializable]
    public class ParagraphComponentList : Collection<IParagraphComponent>, IParagraphComponent, IRecursiveEnumerable
    {
        public static ParagraphComponentList NULL = new ParagraphComponentList();

        #region IParagraphComponent Interface

        public string Text
        {
            get
            {
                var textString = new StringBuilder();

                foreach (IParagraphComponent component in this)
                {
                    textString.Append(component.Text);
                }

                return textString.ToString();
            }
        }

        public string ToXml()
        {
            var xmlString = new StringBuilder();

            foreach (IParagraphComponent component in this)
            {
                xmlString.Append(component.ToXml());
            }

            return xmlString.ToString();
        }

        public string ToHtml()
        {
            var htmlString = new StringBuilder();
            foreach (IParagraphComponent component in this)
            {
                htmlString.Append(component.ToHtml());
            }
            return htmlString.ToString();
        }

        public string ToRtf()
        {
            var rtfString = new StringBuilder();

            foreach (IParagraphComponent component in this)
            {
                rtfString.Append(component.ToRtf());
            }

            return rtfString.ToString();
        }

        public string ToRtf(RtfDocument document)
        {
            var rtfString = new StringBuilder();

            foreach (IParagraphComponent component in this)
            {
                rtfString.Append(component.ToRtf());
            }

            return rtfString.ToString();
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public IEnumerable RecursiveEnumerator
        {
            get
            {
                foreach (IParagraphComponent item in Items)
                {
                    foreach (IParagraphComponent component in item.RecursiveEnumerator)
                    {
                        yield return component;
                    }
                }
            }
        }

        #endregion
    }
}