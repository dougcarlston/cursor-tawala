// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements single SET line for display in Process window.
    /// </summary>
    [Serializable]
    public class ArithmeticLine : ProcessLine
    {
        public ArithmeticLine()
        {
        }

        public ArithmeticLine(ArithmeticStatement statement) : base(statement, statement.ToString())
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
            Boolean isValid = true;

            var arithmeticStatement = Statement as ArithmeticStatement;

            // fieldString and expressionString are always required
            if (arithmeticStatement.Variable == null || arithmeticStatement.Variable.Equals("") || arithmeticStatement.Value.Text == null ||
                arithmeticStatement.Value.Text.Equals(""))
            {
                isValid = false;
            }

            // check for valid variable name
            Process parentProcess = Project.Current.GetProcessOrSkipInstructions(arithmeticStatement);
            int lineIndex = parentProcess.Lines.GetIndex(arithmeticStatement);

            //FieldList variableFieldResolver = parentProcess.GetValidVariables(lineIndex);
            //if (variableFieldResolver[arithmeticStatement.Variable] == null)
            //{
            //    isValid = false;
            //}
            if (Project.Current.AllVariables[arithmeticStatement.Variable] == null)
            {
                isValid = false;
            }

            // if value is a field...
            if (arithmeticStatement.Value.Type == FieldOrLiteral.StringType.field)
            {
                // if this statement's field name does not appear in field list...
                if (fieldResolver[arithmeticStatement.Value.Text] == null)
                {
                    isValid = false;
                }
            }

            return isValid;
        }
    }
}