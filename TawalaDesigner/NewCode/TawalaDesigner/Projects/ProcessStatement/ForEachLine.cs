// $Workfile: ForEachLine.cs $
// $Revision: 7 $	$Date: 5/10/07 11:58p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements top line of FOR EACH statement for display in Process window.
	/// </summary>
	[Serializable]
	public abstract class ForEachLine : ProcessLine
	{
		protected ForEachLine(ForEachStatement statement) : base(statement, statement.ToString())
		{
			this.group = statement;
			this.selectsGroup = true;
			this.isDeletable = true;
			this.isSelectable = true;
			this.canInsertBefore = true;
		}

		/// <summary>
		/// Boolean indicating whether this line is valid.
		/// </summary>
		public override bool IsValidLine(FieldList fieldResolver)
		{
			return true;
		}
	}

	/// <summary>
	/// Implements top line of FOR EACH statement for display in Process window.
	/// </summary>
	[Serializable]
	public class ForEachRecordLine : ForEachLine
	{
		public ForEachRecordLine(ForEachRecordStatement statement)
			: base(statement)
		{
			this.selectsGroup = true;
			this.isDeletable = true;
			this.isSelectable = true;
			this.canInsertBefore = true;
		}

		/// <summary>
		/// Boolean indicating whether this line is valid.
		/// </summary>
		public override bool IsValidLine(FieldList fieldResolver)
		{
			return true;
		}
	}

	#region ForEachQuestionLine (and statement) unused
#if false
	/// <summary>
	/// Implements top line of FOR EACH statement for display in Process window.
	/// </summary>
	[Serializable]
	public class ForEachQuestionLine : ForEachLine
	{
		public ForEachQuestionLine(ForEachQuestionStatement statement) : base(statement)
		{
			this.selectsGroup = true;
			this.isDeletable = true;
			this.isSelectable = true;
			this.canInsertBefore = true;
		}

		/// <summary>
		/// Boolean indicating whether this line is valid.
		/// </summary>
		public override bool IsValidLine(FieldList fieldResolver)
		{
			return true;
		}
	}
#endif
	#endregion
}
