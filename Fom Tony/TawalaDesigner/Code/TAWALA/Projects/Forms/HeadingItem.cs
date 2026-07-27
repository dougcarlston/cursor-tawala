// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Forms;
using Tawala.Projects.Properties;
using Tawala.XmlSupport;

namespace Tawala.Projects
{
    [Serializable]
    public class HeadingItem : TextItem, IHeadingItem
    {
        private const string defaultLabelPrefix = "H";
        private const string tagName = "heading";
        private HeadingType typeOfHeading = HeadingType.Main;

        public HeadingItem() : base(Resources.HeadingItemDefaultRTF)
        {
        }

        public HeadingItem(IXmlElement element) : base(element)
        {
            HeadingType = (HeadingType)(Enum.Parse(typeof(HeadingType), element.GetAttribute("type")));
        }

        protected override string XmlTagName { get { return tagName; } }

        #region IHeadingItem Members

        public HeadingType HeadingType { get { return typeOfHeading; } set { typeOfHeading = value; } }

        public override bool IsTextItem { get { return false; } }

        public override string DefaultLabelPrefix { get { return defaultLabelPrefix; } }

        #endregion

        protected override string GetXmlAttributes(string label)
        {
            return string.Format(" label=\"{0}\"{1} type=\"{2}\"", label, GetAlternateLabelXml(), HeadingType);
        }
    }

    public enum HeadingType
    {
        Main,
        Sub
    } ;
}