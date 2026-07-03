// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements top line of IF statement for display in Process window.
    /// </summary>
    [Serializable]
    public class IfLine : ProcessLine
    {
        public IfLine(IfStatement statement) : base(statement, statement.ToString())
        {
            Group = statement;
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
            var isValid = false;

            var ifStatement = Statement as IfStatement;

            if (ifStatement.IsSimple)
            {
                isValid = true;

                int id = ((Condition)ifStatement.Conditions[0]).Field.Id;

                if (!isProjectField(id))
                {
                    isValid = false;
                }

                var expression = ((Condition)ifStatement.Conditions[0]).Expression;

                if (expression != null && expression.HasSingleFieldElement)
                {
                    IField field = ((FieldElement)expression.Elements[0]).Field;

                    if (!(field is ChoiceField))
                    {
                        if (!isProjectField(field.Id))
                        {
                            isValid = false;
                        }
                    }
                }

                if (!isValid)
                {
                    // if this statement's field name appears in field list...
                    if (fieldResolver[((Condition)ifStatement.Conditions[0]).Field.FieldName] != null)
                    {
                        isValid = true;
                    }
                }
            }
            else if (ifStatement.Conditions.Count > 0)
            {
                isValid = true;

                foreach (var component in ifStatement.Conditions)
                {
                    if (component is Condition)
                    {
                        // if field name does not appear in field list...
                        var field = ((Condition)component).Field;
                        if (fieldResolver[field.FieldName] == null)
                        {
                            isValid = false;
                        }
                    }
                }
            }

            return isValid;
        }

        /// <summary>
        /// Indicates whether the specified field belongs to the current Project.
        /// </summary>
        private static bool isProjectField(int fieldId)
        {
            return Project.FieldMapById.ContainsKey(fieldId);
        }
    }
}