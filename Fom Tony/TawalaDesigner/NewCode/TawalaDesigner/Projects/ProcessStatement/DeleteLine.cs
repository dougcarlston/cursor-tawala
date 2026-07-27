// $Workfile: DeleteLine.cs $
// $Revision: 2 $	$Date: 5/03/07 2:31p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements single Delete line for display in Process window.
	/// </summary>
	[Serializable]
	public class DeleteLine : ProcessLine
	{
		public DeleteLine()
		{
		}

		public DeleteLine(DeleteStatement statement)
			: base(statement, statement.ToString())
		{
			this.selectsGroup = false;
			this.isDeletable = true;
			this.isSelectable = true;
			this.canInsertBefore = true;
		}

		/// <summary>
		/// Boolean indicating whether this line is valid.
		/// </summary>
		public override bool IsValidLine(FieldList fieldResolver)
		{
			DeleteStatement deleteStatement = statement as DeleteStatement;

			if (deleteStatement == null || deleteStatement.Form == null)
				return false;

			return Project.Current.FormList.Contains(deleteStatement.Form);
		}
	}
}
