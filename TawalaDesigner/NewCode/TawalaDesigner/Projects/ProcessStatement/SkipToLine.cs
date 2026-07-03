// $Workfile: SkipToLine.cs $
// $Revision: 5 $	$Date: 5/24/06 7:20p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements single SKIP line for display in Process window.
	/// </summary>
	[Serializable]
	public class SkipToLine : ProcessLine
	{
		public SkipToLine()
		{
		}

		public SkipToLine(SkipToStatement statement) : base (statement, statement.ToString())
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
			SkipToStatement skipToStatement = statement as SkipToStatement;

			Boolean isValid = false;

			if (skipToStatement.Destination != null && skipToStatement.Destination.Valid)
			{
				isValid = true;
			}

			return isValid;
		}
	}
}