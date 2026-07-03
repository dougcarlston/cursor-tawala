// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Documents;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements single SHOW line for display in Process window.
    /// </summary>
    [Serializable]
    public class ShowLine : ProcessLine
    {
        public ShowLine()
        {
        }

        public ShowLine(ShowStatement statement) : base(statement, statement.ToString())
        {
            SelectsGroup = false;
            IsDeletable = true;
            IsSelectable = true;
            CanInsertBefore = true;
        }

        public override bool IsValidLine(FieldList fieldResolver)
        {
            // override in derived classes
            return false;
        }
    }

    [Serializable]
    public class ShowDocumentLine : ShowLine
    {
        public ShowDocumentLine()
        {
        }

        public ShowDocumentLine(ShowDocumentStatement statement) : base(statement)
        {
            SelectsGroup = false;
            IsDeletable = true;
            IsSelectable = true;
            CanInsertBefore = true;

            Project.Events.DocumentChanged += events_DocumentChanged;
        }

        /// <summary>
        /// Boolean indicating whether this line is valid.
        /// </summary>
        public override bool IsValidLine(FieldList fieldResolver)
        {
            var showDocumentStatement = Statement as ShowDocumentStatement;

            Boolean isValid = false;

            if (showDocumentStatement.Document != null)
            {
                isValid = (showDocumentStatement.ValidateDocumentReference(showDocumentStatement.Document) ==
                           ProcessStatement.StatementStatus.Valid);
            }

            return isValid;
        }

        private void events_DocumentChanged(object sender, ComponentEventArgs e)
        {
            var showDocumentStatement = Statement as ShowDocumentStatement;

            if (!Project.Current.DocumentList.Contains(showDocumentStatement.Document))
            {
                showDocumentStatement.Document = Document.NULL;
            }
        }
    }

    [Serializable]
    public class ShowFormLine : ShowLine
    {
        public ShowFormLine()
        {
        }

        public ShowFormLine(ShowFormStatement statement) : base(statement)
        {
            SelectsGroup = false;
            IsDeletable = true;
            IsSelectable = true;
            CanInsertBefore = true;

            Project.Events.ComponentRemoved += events_ComponentRemoved;
        }

        /// <summary>
        /// Boolean indicating whether this line is valid.
        /// </summary>
        public override bool IsValidLine(FieldList fieldResolver)
        {
            var showStatement = Statement as ShowStatement;

            Boolean isValid = false;

            if (showStatement.Form != null)
            {
                isValid = (showStatement.ValidateFormReference(showStatement.Form) == ProcessStatement.StatementStatus.Valid);
            }

            return isValid;
        }

        private void events_ComponentRemoved(object sender, ComponentEventArgs e)
        {
            if (e.Component is IForm)
            {
                var showStatement = Statement as ShowStatement;

                if (showStatement.Form == e.Component as IForm)
                {
                    showStatement.Form = Form.NULL;
                }
            }
        }
    }
}