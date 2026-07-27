// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Processes
{
    [Serializable]
    public class ShowRecordLine : ShowLine
    {
        public ShowRecordLine()
        {
        }

        public ShowRecordLine(ShowRecordStatement statement) : base(statement)
        {
            SelectsGroup = false;
            IsDeletable = true;
            IsSelectable = true;
            CanInsertBefore = true;

            Project.Events.ComponentRemoved += events_ComponentRemoved;
        }

        public override bool IsValidLine(FieldList fieldResolver)
        {
            var showStatement = Statement as ShowStatement;

            bool isValid = false;

            if (showStatement.Form != null)
            {
                isValid = (showStatement.ValidateFormReference(showStatement.Form) == ProcessStatement.StatementStatus.Valid);
            }

            return isValid;
        }

        private void events_ComponentRemoved(object sender, ComponentEventArgs e)
        {
            if (e.Component is IForm)
            {
                var showStatement = Statement as ShowStatement;

                if (showStatement.Form == e.Component as IForm)
                {
                    showStatement.Form = Form.NULL;
                }
            }
        }
    }
}