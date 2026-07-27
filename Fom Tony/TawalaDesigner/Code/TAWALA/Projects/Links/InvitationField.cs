// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.RtfSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects.Links
{
    [Serializable]
    public sealed class InvitationField : ILink
    {
        private const string encodedFieldPrefix = "490046002400"; // "IF$"

        private const string privateFormat =
            "<invitation form=\"{0}\" project=\"{1}\" private=\"true\"><authenticationTokenValue>{2}</authenticationTokenValue><displayText>{3}</displayText></invitation>";

        private const string publicFormat = "<invitation form=\"{0}\" project=\"{1}\"><displayText>{2}</displayText></invitation>";
        public static InvitationField Null = staticInitializer();

        public InvitationField()
        {
            InitialFormName = "";
            Form = Projects.Form.NULL;
            ProjectName = "";
            Id = Project.NextUniqueID;
            Project.InvitationMapById.AddUnique(this);
        }

        public InvitationField(IXmlElement element) : this()
        {
            FieldList fieldResolver = buildFieldResolver();

            DisplayTextExpression = element.HasChild("displayText")
                                        ? new FieldsAndLiteralsExpression(element.GetChild("displayText"))
                                        : new FieldsAndLiteralsExpression(element.Text);

            InitialFormName = element.GetAttribute("form");
            ProjectName = element.GetAttribute("project");
            IsPrivate = element.HasAttribute("private") && element.GetAttribute("private").CompareTo("true") == 0;

            if (IsPrivate)
            {
                if (element.HasChild("authenticationTokenValue"))
                {
                    AuthenticationTokenExpression = new FieldsAndLiteralsExpression(element.GetChild("authenticationTokenValue"));
                }
            }

            if (invitationReferencesCurrentProject())
            {
                Form = makeFormReference();
            }
        }

        public IForm Form { get; set; }

        public string InitialFormName { get; set; }

        public string FormName
        {
            get
            {
                if (Form == Projects.Form.NULL || Form == null)
                {
                    return InitialFormName;
                }

                return Form.Name;
            }
        }

        public string ProjectName { get; set; }

        public bool IsPrivate { get; set; }

        public FieldsAndLiteralsExpression AuthenticationTokenExpression { get; set; }
        public FieldsAndLiteralsExpression DisplayTextExpression { get; set; }

        #region ILink Members

        public int Id { get; private set; }

        public string DisplayText
        {
            get { return DisplayTextExpression.ToString(); }
            set { DisplayTextExpression = new FieldsAndLiteralsExpression(value); }
        }

        public string ToXml()
        {
            if (IsPrivate)
            {
                return String.Format(privateFormat, FormName, ProjectName, AuthenticationTokenExpression.ToXml(),
                                     DisplayTextExpression.ToXml());
            }
            return String.Format(publicFormat, FormName, ProjectName, DisplayTextExpression.ToXml());
        }

        public string ToRtf()
        {
            string rtfString = @"{{\*\txfieldstart\txfieldtype0\txfieldflags" + RtfUtility.NonEditableFieldFlags + @"\txfielddataval{0}" +
                               @"\txfielddata " + encodedFieldPrefix + "{1}}}" + @"{2}{{" + @"\*\txfieldend}}";

            return String.Format(rtfString, Id, RtfUtility.EncodeHexString(Id.ToString()), RtfUtility.EscapeSpecialCharacters(DisplayText));
        }

        public string DesignerDisplayText
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        private static InvitationField staticInitializer()
        {
            var nullInivitation = new InvitationField();
            Project.InvitationMapById.Remove(nullInivitation.Id);
            return nullInivitation;
        }

        private static FieldList buildFieldResolver()
        {
            var allFields = new FieldList();

            foreach (IForm form in Project.Current.FormList)
            {
                allFields.Add(form.GetAllFields());
            }

            allFields.Add(Project.Current.AllVariables);

            return allFields;
        }

        private IForm makeFormReference()
        {
            return Project.Current.GetForm(InitialFormName) ?? Projects.Form.NULL;
        }

        private bool invitationReferencesCurrentProject()
        {
            return ProjectName.Equals("");
        }
    }
}