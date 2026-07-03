// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;

namespace Tawala.Common
{
    /// <summary>
    /// Alphabetic label for blanks and choices.
    /// </summary>
    public class AlphaLabel
    {
        /// <summary>
        /// Label index
        /// </summary>
        protected int index;

        public AlphaLabel(int index)
        {
            this.index = index;
        }

        /// <summary>
        /// Return label id as string
        /// </summary>
        public override string ToString()
        {
            return (indexToString());
        }

        /// <summary>
        /// Convert index to string
        /// </summary>
        private string indexToString()
        {
            var s = new StringBuilder();

            // get single-character value (0-25)
            int cv = index%26;

            // get width of label
            int width = index/26 + 1;

            for (int i = 0; i < width; i++)
            {
                // append label character to result string
                s.Append(nToChar(cv));
            }

            return (s.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        private char nToChar(int x)
        {
            // convert number to character, 0 = lowercase 'a' (97)
            int n = 97 + x;
            return (Convert.ToChar(n));
        }
    }
}