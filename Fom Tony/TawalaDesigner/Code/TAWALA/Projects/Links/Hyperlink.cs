// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.RtfSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects.Links
{
    [Serializable]
    public sealed class Hyperlink : ILink
    {
        private const string designerDisplayTextWhenDisplayTextEmpty = "(Link appears here)";
        private const string encodedFieldPrefix = "480046002400"; // "HF$"

        private static readonly Hyperlink nullLink;
        private FieldsAndLiteralsExpression displayTextExpression;
        private FieldsAndLiteralsExpression fieldsAndLiteralsExpression;

        static Hyperlink()
        {
            const string nullString = "Null";
            nullLink = new Hyperlink
                       {
                           DisplayText = nullString,
                           Url = nullString
                       };
        }

        public Hyperlink()
        {
            Conditions = null;
            Url = string.Empty;
            DisplayText = string.Empty;
            Id = Project.NextUniqueID;
            Project.InvitationMapById.AddUnique(this);
        }

        /// <summary>
        /// Constructs a Hyperlink object from a &lt;link&gt; element
        /// </summary>
        public Hyperlink(IXmlElement element) : this()
        {
            displayTextExpression = new FieldsAndLiteralsExpression(element.GetChild("description"));
            fieldsAndLiteralsExpression = new FieldsAndLiteralsExpression(element.GetChild("url"));
            OpenNewWindow = element.HasChild("new-window");

            if (element.HasChild("displayConditions"))
            {
                IXmlElement conditionsElement = element.GetChild("displayConditions");
                Conditions = new Conditions(conditionsElement, FieldUtil.GetGlobalFieldList(conditionsElement));
            }
        }

        public static Hyperlink NULL
        {
            get
            {
                if (!Project.InvitationMapById.ContainsKey(nullLink.Id))
                {
                    Project.InvitationMapById.AddUnique(nullLink);
                }
                return nullLink;
            }
        }

        /// <summary>
        /// The URL for the hyperlink as a string
        /// </summary>
        public string Url { get { return fieldsAndLiteralsExpression.ToString(); } set { fieldsAndLiteralsExpression = new FieldsAndLiteralsExpression(value); } }

        public Conditions Conditions { get; set; }

        /// <summary>
        /// Specifies whether the hyperlink opens a new browser window
        /// </summary>
        public bool OpenNewWindow { get; set; }

        #region ILink Members

        public int Id { get; protected set; }

        /// <summary>
        /// The text for the hyperlink as seen by the user
        /// </summary>
        public string DisplayText { get { return displayTextExpression.ToString(); } set { displayTextExpression = new FieldsAndLiteralsExpression(value); } }

        public string ToXml()
        {
            var xmlString = new StringBuilder();

            xmlString.Append("<link>" + Environment.NewLine);

            if (OpenNewWindow)
            {
                xmlString.AppendLine("<new-window/>");
            }

            xmlString.AppendLine("<description>");
            xmlString.Append(displayTextExpression.ToXml());
            xmlString.AppendLine("</description>");
            xmlString.AppendLine("<url>");
            xmlString.Append(fieldsAndLiteralsExpression.ToXml());
            xmlString.AppendLine("</url>");

            if (Conditions != null)
            {
                xmlString.Append(Conditions.ToXml("displayConditions"));
            }

            xmlString.Append("</link>" + Environment.NewLine);

            return xmlString.ToString();
        }

        public string ToRtf()
        {
            string rtfString = @"{{\*\txfieldstart\txfieldtype0\txfieldflags" + RtfUtility.NonEditableFieldFlags + @"\txfielddataval{0}" +
                               @"\txfielddata " + encodedFieldPrefix + "{1}}}" + @"{2}{{" + @"\*\txfieldend}}";

            return String.Format(rtfString, Id, RtfUtility.EncodeHexString(Id.ToString()),
                                 RtfUtility.EscapeSpecialCharacters(DesignerDisplayText));
        }

        public string DesignerDisplayText { get { return string.IsNullOrEmpty(DisplayText) ? designerDisplayTextWhenDisplayTextEmpty : DisplayText; } }

        #endregion

        public static Hyperlink Duplicate(Hyperlink link)
        {
            var newLink = new Hyperlink();
            newLink.displayTextExpression = link.displayTextExpression;
            newLink.Conditions = link.Conditions;
            newLink.OpenNewWindow = link.OpenNewWindow;
            newLink.fieldsAndLiteralsExpression = link.fieldsAndLiteralsExpression;
            Project.InvitationMapById.AddUnique(newLink);
            return newLink;
        }
    }
}