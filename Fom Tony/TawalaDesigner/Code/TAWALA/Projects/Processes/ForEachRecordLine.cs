// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements top line of FOR EACH statement for display in Process window.
    /// </summary>
    [Serializable]
    public class ForEachRecordLine : ForEachLine
    {
        public ForEachRecordLine(ForEachRecordStatement statement) : base(statement)
        {
            SelectsGroup = true;
            IsDeletable = true;
            IsSelectable = true;
            CanInsertBefore = true;
        }

        /// <summary>
        /// Boolean indicating whether this line is valid.
        /// </summary>
        public override bool IsValidLine(FieldList fieldResolver)
        {
            return true;
        }
    }
}