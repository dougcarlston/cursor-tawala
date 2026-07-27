// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using Tawala.Common;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms
{
    /// <summary>
    /// Collection of Blank objects
    /// </summary>
    [Serializable]
    public class BlankList : Collection<Blank>
    {
        private int blankIndex;

        /// <summary>
        /// Gets and sets the current blank index, which is used by the MakeBlank method to determine whether to
        /// return an existing Blank, or to create a new one.
        /// </summary>
        public int BlankIndex { get { return blankIndex; } set { blankIndex = value; } }

        /// <summary>
        /// Get label for a blank at the given position in the list.
        /// </summary>
        public string GetLabel(int index)
        {
            if (index >= 0)
            {
                // if an alternate label exists...
                if (this[index].AlternateLabel != "")
                {
                    // return alternate label
                    return this[index].AlternateLabel;
                }
                else
                {
                    // return default label
                    return new AlphaLabel(index).ToString();
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get label for the specified blank in the list.
        /// </summary>
        public string GetLabel(Blank blank)
        {
            return GetLabel(IndexOf(blank));
        }

        /// <summary>
        /// Factory method that returns a Blank object based on the specified XML element.
        /// If the current blankIndex corresponds to a blank that already exists in the list, that blank is returned.
        /// Otherwise, a new blank is created and added to the list, then returned.
        /// </summary>
        public Blank MakeBlank(IXmlElement element, FibItem owner)
        {
            var newBlank = new Blank(element, owner);

            if (blankIndex < Count)
            {
                Blank existingBlank = this[blankIndex];

                existingBlank.Length = newBlank.Length;

                existingBlank.AlternateLabel = newBlank.AlternateLabel;

                existingBlank.Required = newBlank.Required;

                existingBlank.ValidationFunction = newBlank.ValidationFunction;

                return existingBlank;
            }
            Add(newBlank);

            return newBlank;
        }

        /// <summary>
        /// Truncates the list, by removing Blanks from the end, so that it contains the specified count of Blanks.
        /// </summary>
        public void Truncate(int count)
        {
            while (Count > count)
            {
                RemoveAt(Count - 1);
            }
        }
    }
}