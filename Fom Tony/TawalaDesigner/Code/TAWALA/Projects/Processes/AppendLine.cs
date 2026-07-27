// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Documents;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements single APPEND line for display in Process window.
    /// </summary>
    [Serializable]
    public class AppendLine : ProcessLine
    {
        public AppendLine()
        {
        }

        public AppendLine(AppendStatement statement) : base(statement, statement.ToString())
        {
            SelectsGroup = false;
            IsDeletable = true;
            IsSelectable = true;
            CanInsertBefore = true;
        }

        /// <summary>
        /// Boolean indicating whether this line is valid.
        /// </summary>
        public override bool IsValidLine(FieldList fieldResolver)
        {
            var appendStatement = Statement as AppendStatement;

            Boolean isValid = appendStatement.Document != Document.NULL && appendStatement.Appendage != Document.NULL &&
                              appendStatement.ValidateDocumentReference(appendStatement.Document) == ProcessStatement.StatementStatus.Valid &&
                              appendStatement.ValidateDocumentReference(appendStatement.Appendage) == ProcessStatement.StatementStatus.Valid;

            return isValid;
        }
    }
}