// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements single SHOW line for display in Process window.
    /// </summary>
    [Serializable]
    public class ShowUrlLine : ProcessLine
    {
        public ShowUrlLine()
        {
        }

        public ShowUrlLine(ShowUrlStatement statement) : base(statement, statement.ToString())
        {
            SelectsGroup = false;
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