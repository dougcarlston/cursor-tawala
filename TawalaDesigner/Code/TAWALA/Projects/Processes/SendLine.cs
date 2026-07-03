// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements single SEND line for display in Process window.
    /// </summary>
    [Serializable]
    public sealed class SendLine : ProcessLine
    {
        public SendLine()
        {
        }

        public SendLine(SendStatement statement) : base(statement, statement.ToString())
        {
            SelectsGroup = false;
            IsDeletable = true;
            IsSelectable = true;
            CanInsertBefore = true;
        }

        private bool isEmpty(string textString)
        {
            return (string.IsNullOrEmpty(textString));
        }

        /// <summary>
        /// Boolean indicating whether this line is valid.
        /// </summary>
        public override bool IsValidLine(FieldList fieldResolver)
        {
            var isValid = true;

            var sendStatement = Statement as SendStatement;

            // To: address and Subject line are always required
            if (isEmpty(sendStatement.AddressTo.Text) || isEmpty(sendStatement.Subject))
            {
                isValid = false;
            }
            else
            {
                isValid = sendStatement.SendBody.IsValid(sendStatement);
            }

            if (sendStatement.AddressTo.Type == FieldOrLiteral.StringType.field)
            {
                // if this statement's to address does not appear in field list...
                if (fieldResolver[sendStatement.AddressTo.Text] == null)
                {
                    isValid = false;
                }
            }

            return isValid;
        }
    }
}