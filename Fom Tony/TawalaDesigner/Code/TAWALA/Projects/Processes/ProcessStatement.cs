// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using Tawala.Projects.Documents;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Base class for Process statements.
    /// </summary>
    [Serializable]
    public class ProcessStatement : IProcessStatement
    {
        #region StatementStatus enum

        /// <summary>
        /// Context-based status of statement.
        /// </summary>
        public enum StatementStatus
        {
            Valid,
            Invalid
        }

        #endregion

        public static IProcessStatement NULL = new ProcessStatement();

        /// <summary>
        /// Statement name
        /// </summary>
        protected string name;

        public string Name { get { return name; } }

        // return XML representation of statement

        #region IProcessStatement Members

        public virtual string ToXml()
        {
            return "";
        }

        #endregion

        public ProcessStatement Copy()
        {
            return (ProcessStatement)MemberwiseClone();
        }

        public StatementStatus ValidateDocumentReference(IDocument document)
        {
            StatementStatus stat = StatementStatus.Invalid;

            if (documentExists(document))
            {
                FieldList documentFields = document.GetFields();
                if (documentFields.Count == 0)
                {
                    stat = StatementStatus.Valid;
                }
                else
                {
                    stat = allFieldsInDocumentAreValid(documentFields);
                }
            }

            return stat;
        }

        private static bool documentExists(IDocument document)
        {
            return Project.Current.RealOrVirtualDocumentList.Contains(document);
        }

        private static StatementStatus allFieldsInDocumentAreValid(FieldList documentFields)
        {
            FieldList allFields = getAllFieldsInProject();
            if (allFields.ContainsAll(documentFields))
            {
                return StatementStatus.Valid;
            }

            return StatementStatus.Invalid;
        }

        private static FieldList getAllFieldsInProject()
        {
            var allFields = new FieldList();
            foreach (IForm form in Project.Current.FormList)
            {
                allFields.Add(form.GetAllFields());
            }

            allFields.Add(Project.Current.AllVariables);
            return allFields;
        }

        /// <summary>
        /// Checks a reference to a Form for validity
        /// </summary>
        /// <param name="document">The Form to check</param>
        /// <returns>StatementStatus (Valid or Invalid)</returns>
        /// <remarks>
        /// This method returns the status of this statement based on the
        /// following conditions:
        /// 
        /// Status		Conditions
        /// ======		==========
        /// 
        /// Valid -		Statement references no document.
        /// 
        /// Invalid -	Statement references a document.
        ///				Form does not exist in the Project's form list.
        ///				
        /// Valid -		Statement references a form.
        ///				Form exists in the Project's form list.
        /// </remarks>
        public StatementStatus ValidateFormReference(IForm form)
        {
            StatementStatus stat = StatementStatus.Valid;
            IForm listForm = Project.Current.GetForm(form.Name);

            // check for absence of statement's form from form list
            if (listForm == null)
            {
                // flag statement as invalid
                stat = StatementStatus.Invalid;
            }
            else
            {
                // if names are the same, but are different references...
                if (form != listForm)
                {
                    stat = StatementStatus.Invalid;
                    Debug.Assert(false, "This shouldn't happen!!!");
                }
            }

            return stat;
        }

        public virtual IProcessElement AsProcessElement()
        {
            return null;
        }

        public virtual Type GetStatementType()
        {
            return GetType();
        }
    }
}