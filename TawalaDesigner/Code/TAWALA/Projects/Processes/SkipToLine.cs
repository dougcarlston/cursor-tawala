// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements single SKIP line for display in Process window.
    /// </summary>
    [Serializable]
    public class SkipToLine : ProcessLine
    {
        public SkipToLine()
        {
        }

        public SkipToLine(SkipToStatement statement) : base(statement, statement.ToString())
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
            var skipToStatement = Statement as SkipToStatement;

            Boolean isValid = false;

            if (skipToStatement.Destination != null && skipToStatement.Destination.Valid)
            {
                isValid = true;
            }

            return isValid;
        }
    }
}