// $Workfile: ShowUrlLine.cs $
// $Revision: 2 $	$Date: 2/28/08 2:01p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements single SHOW line for display in Process window.
	/// </summary>
	[Serializable]
	public class ShowUrlLine : ProcessLine
	{
		public ShowUrlLine()
		{
		}

		public ShowUrlLine(ShowUrlStatement statement) : base(statement, statement.ToString())
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
