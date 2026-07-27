// $Workfile: OtherwiseLine.cs $
// $Revision: 4 $	$Date: 11/25/05 5:00p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements single "otherwise" line (such as that used in IF statements)
	/// for display in Process window.
	/// </summary>
	[Serializable]
	public class OtherwiseLine : ProcessLine
	{
		public OtherwiseLine(ProcessStatement statement, string otherwiseText) : base (null, otherwiseText)
		{
			this.group = statement;
			this.selectsGroup = false;
			this.isDeletable = false;
			this.isSelectable = false;
			this.canInsertBefore = false;
		}

		public override string ToXml()
		{
			return "";
		}

	}
}
