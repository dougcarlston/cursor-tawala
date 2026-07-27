// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements single SET line for display in Process window.
    /// </summary>
    [Serializable]
    public class GetLine : ProcessLine
    {
        public GetLine()
        {
        }

        public GetLine(GetStatement statement) : base(statement, statement.ToString())
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
            var getStatement = Statement as GetStatement;

            if (getStatement == null || getStatement.Records == null || getStatement.Records.Forms == null)
            {
                return false;
            }

            int validFormCount = 0;

            FormList allForms = Project.Current.AllForms;

            foreach (IForm f in getStatement.Records.Forms)
            {
                if (allForms.Contains(f))
                {
                    ++validFormCount;
                }
            }

            return validFormCount > 0;
        }
    }
}