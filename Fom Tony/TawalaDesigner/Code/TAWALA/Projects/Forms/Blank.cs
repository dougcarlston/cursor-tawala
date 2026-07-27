// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Text;
using Tawala.Common;
using Tawala.Functions.Runtime;
using Tawala.Projects.Documents;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.XmlSupport;

namespace Tawala.Projects.Forms
{
    /// <summary>
    /// Class to contain a single blank in a fill-in-the-blank question
    /// </summary>
    [Serializable]
    public class Blank : ParagraphComponent, IDeserializedField, IAssignableField, IBlank
    {
        private const string label = "a";
        private readonly int id = Project.NextUniqueID;
        private string alternateLabel = "";
        private int height = 1;

        // length of blank in characters
        private int length;
        private FibItem owner;

        [NonSerialized]
        private IXmlElement validationFunctionElement;

        private string validationFunctionXml;

        public Blank(FibItem owner, int length)
        {
            this.owner = owner;
            this.length = length;

            Project.FieldMapById.AddUnique(this);
        }

        public Blank(IXmlElement element, FibItem owner)
        {
            this.owner = owner;
            //this.label = element.GetAttribute("label");
            length = Convert.ToInt32(element.GetAttribute("length"));
            if (element.HasAttribute("height"))
            {
                height = Convert.ToInt32(element.GetAttribute("height"));
            }
            Required = (element.GetAttribute("required") == "true");

            if (element.HasAttribute("alternateLabel"))
            {
                alternateLabel = element.GetAttribute("alternateLabel");
            }

            if (element.HasChild("validator"))
            {
                //XmlToFunctionConverter converter = new XmlToFunctionConverter();
                validationFunctionElement = new XmlElement(element.GetChild("validator").InnerXml);
                //ValidationFunction = converter.ConvertFrom(validationFunctionElement);
            }

            Project.FieldMapById.AddUnique(this);
            Project.Events.ProjectOpened += events_ProjectOpened;
        }

        public FibItem Owner
        {
            get
            {
                if (owner != null && !owner.BlankList.Contains(this))
                {
                    owner = null;
                }
                return owner;
            }
        }

        public int Height { get { return height; } set { height = value; } }

        #region IAssignableField Members

        public string AssignmentName { get { return QualifiedFieldName; } }

        #endregion

        #region IBlank Members

        [NonSerialized]
        private IFunction validationFunction;

        public IFunction ValidationFunction
        {
            get { return validationFunction; }
            set { validationFunction = value; }
        }

        // alphabetical label

        public int Length { get { return length; } set { length = value; } }

        // Is answer to blank required?  Defaults to false.

        public bool Required { get; set; }

        // alternate label

        public string AlternateLabel
        {
            get { return alternateLabel; }
            set
            {
                // Tempting, but the following condition prevents the FormItemChangedEvent from being raised when a new
                // blank has been added to a FIB. At this point I can think of only kludgy ways to deal with this, passing
                // a flag all the way upstream from the Forms.FIBItem class.  jdf - 4/20/07
                //if (alternateLabel != value.Trim())
                {
                    Project.FieldMapByName.Remove(this);
                    alternateLabel = value.Trim();
                    Project.FieldMapByName.AddUnique(this);
                    Project.Events.RaiseFormItemChangedEvent(new FormItemEventArgs(null, owner, -1));
                }
            }
        }

        // Height of blank in lines.

        public override string ToString()
        {
            return QualifiedFieldName;
        }

        #endregion

        #region IDeserializedField Members

        public IDeserializedField DeserializedFieldReference { get { return (IDeserializedField)Project.FieldMapById[Id]; } }

        public int Id { get { return id; } }

        #endregion

        private bool hasAlternateLabel()
        {
            return alternateLabel != "";
        }

        /// <summary>
        /// Returns a string of underscores representing the blank element at the specified reader's current position
        /// </summary>
        public string GetUnderscores()
        {
            var blankText = new StringBuilder(length + 2);
            blankText.Append('_', length);
            return blankText.ToString();
        }

        public void Eliminate()
        {
            Project.FieldMapById.Remove(id);
            Project.FieldMapByName.Remove(this);
            owner = null;
        }

        #region IPaletteField Interface

        public string FieldName
        {
            get
            {
                if (AlternateLabel != "")
                {
                    return (AlternateLabel);
                }
                return getFieldName();
            }
        }

        public string QualifiedFieldName
        {
            get
            {
                IForm form;

                if ((form = getFormContaining()) == Form.NULL)
                {
                    return FieldUtil.UnknownFieldName;
                }
                return form.Name + ":" + FieldName;
            }
        }

        public string FieldString { get { return "<<" + QualifiedFieldName + ">>"; } }

        public IField this[string name]
        {
            get
            {
                if (name == FieldName)
                {
                    return this;
                }

                return null;
            }
        }

        private string getFieldName()
        {
            if (Owner != null)
            {
                return Owner.FieldName + ":" + getAlphaLabel();
            }
            return "BlankWithNoOwner";
        }

        private string getAlphaLabel()
        {
            if (Owner != null)
            {
                return new AlphaLabel(Owner.BlankList.IndexOf(this)).ToString();
            }
            return label;
        }

        private IForm getFormContaining()
        {
            if (Owner != null && Project.Current.AllForms.ContainsComponent(Owner.Form))
            {
                return owner.Form;
            }

            return Form.NULL;
        }

        #endregion

        #region IEnumerable Interface

        public override IEnumerator GetEnumerator()
        {
            yield return this;
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public override IEnumerable RecursiveEnumerator { get { yield return this; } }

        #endregion

        #region IOperatorDataSource

        public IList OperatorDataSource { get { return HybridOperator.List.DataSource; } }

        #endregion

        #region IParagraphComponent Interface

        public override string Text { get { return GetUnderscores(); } }

        public override string ToXml()
        {
            var xmlString = new StringBuilder(128);
            xmlString.AppendFormat("<blank label=\"{0}\" length=\"{1}\"", getAlphaLabel(), length);

            if (height > 1)
            {
                xmlString.AppendFormat(" height=\"{0}\"", height);
            }

            xmlString.AppendFormat(" required=\"{0}\"", Convert.ToBoolean(Required).ToString().ToLower());

            if (hasAlternateLabel())
            {
                xmlString.AppendFormat(" alternateLabel=\"{0}\"", XMLStringFormatter.EscapeAttributeText(alternateLabel));
            }

            xmlString.Append(">");

            if (ValidationFunction != null)
            {
                xmlString.AppendFormat("<validator>{0}</validator>", ValidationFunction.ToXml());
            }

            xmlString.Append("</blank>");

            return xmlString.ToString();
        }

        public override string ToRtf()
        {
            return GetUnderscores();
        }

        #endregion

        [OnSerializing]
        private void onSerializingBlank(StreamingContext context)
        {
            if (ValidationFunction != null)
            {
                validationFunctionXml = ValidationFunction.ToXml();
            }
        }

        [OnDeserialized]
        private void onDeserializedBlank(StreamingContext context)
        {
            regenerateValidationFunction();
        }

        private void regenerateValidationFunction()
        {
            if (validationFunctionXml != null)
            {
                validationFunctionElement = new XmlElement(validationFunctionXml);
            }

            if (validationFunctionElement != null)
            {
                var converter = new XmlToFunctionConverter();
                ValidationFunction = converter.ConvertFrom(validationFunctionElement);
            }

            validationFunctionXml = null;
            validationFunctionElement = null;
        }

        private void events_ProjectOpened(object sender, ProjectEventArgs e)
        {
            regenerateValidationFunction();
        }
    }
}