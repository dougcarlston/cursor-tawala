// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements top line of FOR EACH statement for display in Process window.
    /// </summary>
    [Serializable]
    public abstract class ForEachLine : ProcessLine
    {
        protected ForEachLine(ForEachStatement statement) : base(statement, statement.ToString())
        {
            Group = statement;
            SelectsGroup = true;
            IsDeletable = true;
            IsSelectable = true;
            CanInsertBefore = true;
        }

        public override bool IsValidLine(FieldList fieldResolver)
        {
            return true;
        }
    }
}