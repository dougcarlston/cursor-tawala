// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Tawala.Common;
using Tawala.Projects.Documents;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Forms
{
    /// <summary>
    /// Collection of Choice objects
    /// </summary>
    [Serializable]
    public class ChoiceList : Collection<IChoice>, IChoiceList, IField, IRecursiveEnumerable
    {
        private const int MAX_LABEL_INDEX = 77;

        #region IChoiceList Members

        /// <summary>
        /// Get choice label for a choice at the given position in the list.
        /// </summary>
        public string GetLabel(int index)
        {
            return new AlphaLabel(index).ToString();
        }

        public string QualifiedFieldName { get { throw new NotImplementedException(); } }

        public string ToXhtml(IFormItem formItem)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Returns a collection of labels for all the choices in the list.
        /// Used as a datasource when testing multiple choice choices (by label which may change)
        /// </summary>
        public StringCollection GetLabelsCollection()
        {
            var sc = new StringCollection();

            for (int i = 0; i < Count; ++i)
            {
                sc.Add(new AlphaLabel(i).ToString());
            }

            return sc;
        }

        public string ToXml()
        {
            var xmlString = new StringBuilder();

            for (int i = 0; i < Count; i++)
            {
                xmlString.Append(this[i].ToXml(GetLabel(i)));
            }

            return xmlString.ToString();
        }

        public string ToRtf(RtfDocument document)
        {
            var rtfString = new StringBuilder();

            for (int i = 0; i < Count; i++)
            {
                rtfString.Append(this[i].ToRtf(GetLabel(i), document));
            }

            return rtfString.ToString();
        }

        public static int IndexOfLabel(string label)
        {
            for (int i = 0; i <= MAX_LABEL_INDEX; i++)
            {
                var alphaLabel = new AlphaLabel(i);

                if (label.Equals(alphaLabel.ToString()))
                {
                    return i;
                }
            }

            return -1;
        }

        #region IField Interface

        private readonly int id = Project.NextUniqueID;

        public string FieldName { get { return "Unnamed ChoiceList"; } }

        public string FieldString { get { return "<<" + FieldName + ">>"; } }

        public IField this[string name]
        {
            get
            {
                foreach (IField field in RecursiveEnumerator)
                {
                    if (field.FieldName == name)
                    {
                        return field;
                    }
                }

                // field name not found
                return null;
            }
        }

        public int Id { get { return id; } }

        #endregion

        #region IRecursiveEnumerable Interface

        public IEnumerable RecursiveEnumerator
        {
            get
            {
                foreach (string s in GetLabelsCollection())
                {
                    yield return new ChoiceField(s);
                }
            }
        }

        #endregion
    }

    [Serializable]
    public class ChoiceField : Field, IDeserializedField
    {
        public ChoiceField(string name) : base(name)
        {
        }

        #region IDeserializedField Members

        public IDeserializedField DeserializedFieldReference { get { return this; } }

        #endregion
    }
}