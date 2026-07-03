// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements single "otherwise" line (such as that used in IF statements)
    /// for display in Process window.
    /// </summary>
    [Serializable]
    public class OtherwiseLine : ProcessLine
    {
        public OtherwiseLine(ProcessStatement statement, string otherwiseText) : base(null, otherwiseText)
        {
            Group = statement;
            SelectsGroup = false;
            IsDeletable = false;
            IsSelectable = false;
            CanInsertBefore = false;
        }

        public override string ToXml()
        {
            return "";
        }
    }
}