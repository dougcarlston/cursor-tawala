// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;
using Tawala.Projects.Documents;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.RtfSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms
{
    /// <summary>
    /// Summary description for FormItem.
    /// </summary>
    [Serializable]
    public class FormItem : IPaletteField, IFormItem
    {
        private const string attributeFormat = " alternateLabel=\"{0}\"";
        public static readonly FormItem NULL = new FormItem();
        protected static readonly string rtfDefaultTabs = @"\deftab0\tx2880";
        protected static readonly string rtfEnd = @"}";
        protected static readonly string rtfProlog = @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine;

        /// <summary>
        /// User assigned label, must be unique within project,
        /// but no checking is done at this level.
        /// </summary>
        protected string alternateLabel = string.Empty;

        protected RtfColorTable colorTable = new RtfColorTable();
        protected Collection<IDocumentBlock> contents = new Collection<IDocumentBlock>();
        private Conditions displayConditions = new Conditions();
        protected RtfFontTable fontTable = new RtfFontTable();
        protected IForm form;

        protected IFormItemContents newContents;

        [NonSerialized]
        protected RtfParser parser;

        protected string style;
        protected string text;

        public FormItem()
        {
            Id = Project.NextUniqueID;
        }

        protected FormItem(ExternalForm form, string altLabel)
        {
            Id = Project.NextUniqueID;
            alternateLabel = altLabel;
            this.form = form;
            Project.FieldMapById.AddUnique(this);
        }

        ///// <summary>
        ///// List of blocks, such as paragraphs and tables, in FormItem
        ///// </summary>
        public Collection<IDocumentBlock> Contents { get { return contents; } set { contents = value; } }

        public virtual IFormItemContents NewContents { get { return newContents; } set { newContents = value; } }

        public virtual string Text { get { return text; } set { text = value; } }

        #region IFormItem Members

        public bool Selected { get; set; }

        public virtual string ToXml()
        {
            Debug.Assert(false);
            return null;
        }

        /// <summary>
        /// Indicates whether this item is a text item
        /// </summary>
        /// <remarks>
        /// Should be overridden to return true if the item is a text item
        /// </remarks>
        public virtual bool IsTextItem { get { return false; } }

        public virtual bool IsQuestionItem { get { return false; } }

        public virtual string AlternateLabel
        {
            get { return alternateLabel; }
            set
            {
                if (value == null)
                {
                    Project.FieldMapByName.Remove(this);
                    alternateLabel = string.Empty;
                    Project.FieldMapByName.AddUnique(this);
                }
                else
                {
                    if (alternateLabel != value.Trim())
                    {
                        Project.FieldMapByName.Remove(this);

                        alternateLabel = value.Trim();
                        Project.FieldMapByName.AddUnique(this);

                        Project.Events.RaiseFormItemChangedEvent(new FormItemEventArgs(null, this, -1));
                    }
                }
            }
        }

        /// <summary>
        /// Form Item Style String
        /// </summary>
        public virtual string Style { get { return style; } set { style = value; } }

        public Conditions DisplayConditions { get { return displayConditions; } set { displayConditions = (value ?? new Conditions()); } }

        public bool HasDisplayConditions { get { return DisplayConditions.Count > 0; } }

        public void ClearId()
        {
            Id = 0;
        }

        public virtual void Eliminate()
        {
            Project.FieldMapById.Remove(Id);
            Project.FieldMapByName.Remove(this);
        }

        public virtual IForm Form
        {
            get { return form; }

            set
            {
                form = value;

                if (form != null)
                {
                    Project.FieldMapByName.AddUnique(this);
                }
            }
        }

        public void ResolveFieldReferences()
        {
            if (newContents != null)
            {
                newContents.ResolveFieldReferences();
            }
        }

        public void ResolveFunctionReferences()
        {
            if (newContents != null)
            {
                newContents.ResolveFunctionReferences();
            }
        }

        #endregion

        #region IPaletteField Members

        public int Id { get; protected set; }

        #endregion

        /// <summary>
        /// Establish a valid ID for this form item upon deserialization.
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        internal void onDeserialized(StreamingContext context)
        {
            // if item has no id (because it was copied, for example)
            if (Id == 0)
            {
                // assign a new, unique id
                Id = Project.NextUniqueID;
            }
        }

        /// <summary>
        /// Return an XML string representing the form item's "alternateLabel" attribute.
        /// </summary>
        public string GetAlternateLabelXml()
        {
            if (alternateLabel.Length == 0)
            {
                return string.Empty;
            }
            return string.Format(attributeFormat, XMLStringFormatter.EscapeAttributeText(alternateLabel));
        }

        /// <summary>
        /// Called when the object is deserialized but before the graph is returned.
        /// </summary>
        [OnDeserializing]
        private void onDeserializing(StreamingContext context)
        {
            // we never want this to be null, just empty at worst.
            if (alternateLabel == null)
            {
                alternateLabel = string.Empty;
            }
        }

        public virtual string ToRtf()
        {
            var rtfString = new StringBuilder(rtfProlog);

            RtfDocument emptyDocument = getSkeletalRtfDocument();

            rtfString.Append(emptyDocument.FontTable.ToRtf());
            rtfString.Append(emptyDocument.ColorTable.ToRtf());

            rtfString.Append(rtfDefaultTabs);

            foreach (IDocumentBlock block in contents)
            {
                if (block != null)
                {
                    rtfString.Append(block.ToRtf(emptyDocument));
                }
            }

            rtfString.Append(rtfEnd);

            return rtfString.ToString();
        }

        /// <summary>
        /// Returns an RtfDocument with minimal font and color tables.
        /// </summary>
        protected virtual RtfDocument getSkeletalRtfDocument()
        {
            var document = new RtfDocument("Empty Document");

            foreach (RtfFontTableEntry entry in fontTable)
            {
                document.FontTable.Add(entry);
            }

            foreach (RtfColorTableEntry entry in colorTable)
            {
                document.ColorTable.Add(entry);
            }

            return document;
        }

        /// <summary>
        /// Returns a font table built from the &lt;font&gt; elements that are descendants of the specified element. 
        /// </summary>
        protected virtual RtfFontTable getFontTable(IXmlElement element)
        {
            var fontTable = new RtfFontTable();

            Collection<XmlElement> fontElements = element.GetDescendants("font");

            fontTable.AddUnique(new RtfFontTableEntry("swiss", "Arial"));

            foreach (XmlElement fontElement in fontElements)
            {
                fontTable.AddUnique(new RtfFontTableEntry(fontElement));
            }

            return fontTable;
        }

        /// <summary>
        /// Returns a color table built from the &lt;font&gt; elements that are descendants of the specified element. 
        /// </summary>
        protected virtual RtfColorTable getColorTable(IXmlElement element)
        {
            var colorTable = new RtfColorTable();

            Collection<XmlElement> fontElements = element.GetDescendants("font");

            colorTable.AddUnique(new RtfColorTableEntry(0, 0, 0));
            colorTable.AddUnique(new RtfColorTableEntry(255, 255, 255));

            foreach (XmlElement fontElement in fontElements)
            {
                colorTable.AddUnique(new RtfColorTableEntry(fontElement));
            }

            return colorTable;
        }

        protected IForm GetContainingForm()
        {
            return Project.Current.GetFormContaining(this);
        }

        /// <summary>
        /// Regex.Replace callback function to escape special characters in text.
        /// </summary>
        protected static string EscapeSpecialCharacters(Match m)
        {
            if (m.Value.StartsWith("<<"))
            {
                return "<field name=\"" + XMLStringFormatter.EscapeAttributeText(m.Value.Substring(2, m.Value.Length - 4)) + "\"/>";
            }
            switch (m.Value)
            {
                case "&":
                    return "&amp;";
                case "<":
                    return "&lt;";
                case ">":
                    return "&gt;";
            }

            return string.Empty;
        }

        protected string displayConditionsToXml()
        {
            return DisplayConditions == null ? "" : DisplayConditions.ToXml("displayConditions");
        }

        protected void getDisplayConditions(IXmlElement element)
        {
            if (element.HasChild("displayConditions"))
            {
                IXmlElement displayConditionsElement = getDisplayConditionsElementWithoutWhitespace(element);
                DisplayConditions = new Conditions(displayConditionsElement, FieldUtil.GetGlobalFieldList(displayConditionsElement));
            }
        }

        private static IXmlElement getDisplayConditionsElementWithoutWhitespace(IXmlElement element)
        {
            // if the conditions have whitespace in them, it will wreak havoc elsewhere in the code
            IXmlElement rawDisplayConditionsElement = element.GetChild("displayConditions");
            return new XmlElement(rawDisplayConditionsElement.OuterXml, false);
        }

        #region IField Interface

        public virtual string FieldName { get { return ""; } }

        public virtual string QualifiedFieldName { get { return ""; } }

        public virtual string FieldString { get { return "<<" + QualifiedFieldName + ">>"; } }

        public virtual IField this[string name]
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