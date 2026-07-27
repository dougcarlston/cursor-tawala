// $Workfile: ShowRecordLine.cs $
// $Revision: 3 $	$Date: 5/18/07 2:47p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using Tawala.Projects.Forms;

namespace Tawala.Projects
{

	[Serializable]
	public class ShowRecordLine : ShowLine
	{
		public ShowRecordLine()
		{
		}

		public ShowRecordLine(ShowRecordStatement statement)
			: base(statement)
		{
			this.selectsGroup = false;
			this.isDeletable = true;
			this.isSelectable = true;
			this.canInsertBefore = true;

			Project.Events.ComponentRemoved += new EventHandler<ComponentEventArgs>(events_ComponentRemoved);
		}

		public override bool IsValidLine(FieldList fieldResolver)
		{
			ShowStatement showStatement = statement as ShowStatement;

			bool isValid = false;

			if (showStatement.Form != null)
			{
				isValid = (showStatement.ValidateFormReference(showStatement.Form) == ProcessStatement.StatementStatus.Valid);
			}

			return isValid;
		}

		void events_ComponentRemoved(object sender, ComponentEventArgs e)
		{
			if (e.Component is IForm)
			{
				ShowStatement showStatement = statement as ShowStatement;

				if (showStatement.Form == e.Component as IForm)
				{
                    showStatement.Form = NullObjects.Form;
				}
			}
		}
	}
}
