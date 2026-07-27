// $Workfile: BlockCloseLine.cs $
// $Revision: 5 $	$Date: 11/25/05 5:00p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements single "block close" line (such as closing parenthesis)
	/// for display in Process window.
	/// </summary>
	[Serializable]
	public class BlockCloseLine : ProcessLine
	{
		public BlockCloseLine(ProcessStatement statement, string blockOpenText, string xmlString) : base (null, blockOpenText)
		{
			this.group = statement;
			this.selectsGroup = false;
			this.isDeletable = false;
			this.isSelectable = true;
			this.canInsertBefore = true;
			this.xmlString = xmlString;
		}

		protected string xmlString;

		public override string ToXml()
		{
			return xmlString;
		}

	}
}
