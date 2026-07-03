// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using Tawala.Projects.Forms;
using Tawala.Projects.Processes;
using Tawala.Projects.Properties;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    [Serializable]
    public class SkipInstructionsItem : FormItem, ISkipInstructionsItem
    {
        private Process instructions;

        public SkipInstructionsItem()
        {
            instructions = new SkipInstructions();
        }

        /// <summary>
        /// Construct SkipInstructionsItem from XML "&lt;skipInstructions&gt;" element.
        /// </summary>
        public SkipInstructionsItem(IXmlElement element)
        {
            instructions = new SkipInstructions(element);
        }

        #region ISkipInstructionsItem Members

        public Process Instructions { get { return instructions; } set { instructions = value; } }

        public override string ToXml()
        {
            return instructions.ToXml();
        }

        public override string AlternateLabel { get { return string.Empty; } set { } }

        /// <summary>
        /// Get summary of skip destinations for UI.
        /// </summary>
        public string GetSummary()
        {
            if (Instructions.Lines.Count == 0)
            {
                return Resources.SkipSummaryEmpty;
            }
            else
            {
                var destinations = new StringCollection();
                bool unconditionalSkip = true;

                foreach (ProcessLine line in Instructions.Lines)
                {
                    var skip = line.Statement as SkipToStatement;

                    if (skip != null)
                    {
                        if (line.IndentLevel > 0) // REVISIT:  Need to see if within a conditional and may be multiply nested!
                        {
                            unconditionalSkip = false;
                        }

                        string destName = skip.Destination.ToString();
                        if (!destinations.Contains(destName))
                        {
                            destinations.Add(destName);
                        }
                    }
                }

                if (destinations.Count > 1)
                {
                    unconditionalSkip = false;
                }

                if (destinations.Count == 0)
                {
                    return Resources.SkipSummaryNoSkips;
                }
                else if (unconditionalSkip)
                {
                    return string.Format(Resources.SkipSummaryAlwaysSkip, destinations[0]);
                }
                else
                {
                    var sb = new StringBuilder();

                    foreach (string dest in destinations)
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append(", ");
                        }
                        sb.Append(dest);
                    }

                    return string.Format(Resources.SkipSummaryMaySkip, sb);
                }
            }
        }

        #endregion

        #region IField Interface

        public override string FieldName { get { return "Skip Instructions Item"; } }

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
    }
}