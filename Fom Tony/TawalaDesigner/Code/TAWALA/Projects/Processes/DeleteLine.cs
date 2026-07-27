// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements single Delete line for display in Process window.
    /// </summary>
    [Serializable]
    public class DeleteLine : ProcessLine
    {
        public DeleteLine()
        {
        }

        public DeleteLine(DeleteStatement statement) : base(statement, statement.ToString())
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
            var deleteStatement = Statement as DeleteStatement;

            if (deleteStatement == null || deleteStatement.Form == null)
            {
                return false;
            }

            return Project.Current.FormList.Contains(deleteStatement.Form);
        }
    }
}