// $Workfile: CommentLine.cs $
// $Revision: 1 $	$Date: 8/01/06 4:16p $
// Copyright © 2005 - 2006 Tawala Systems, Inc. All rights reserved.

using System;
using System.Text.RegularExpressions;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements single Comment line for display in Process window
	/// </summary>
	[Serializable]
	public class CommentLine : ProcessLine
	{
		public CommentLine(CommentStatement statement)
			: base(statement, statement.ToString())
		{
			this.selectsGroup = false;
			this.isDeletable = true;
			this.isSelectable = true;
			this.canInsertBefore = true;
		}

		public override bool IsValidLine(FieldList fieldResolver)
		{
			return true;
	    }
	}
}
