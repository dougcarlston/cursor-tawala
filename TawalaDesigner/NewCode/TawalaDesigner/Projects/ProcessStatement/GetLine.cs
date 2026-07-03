// $Workfile: GetLine.cs $
// $Revision: 8 $	$Date: 8/23/07 12:24p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using Tawala.Projects.Forms;

namespace Tawala.Projects
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

		public GetLine(GetStatement statement)
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
			GetStatement getStatement = statement as GetStatement;

			if (getStatement == null || getStatement.Records == null || getStatement.Records.Forms == null)
				return false;

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
