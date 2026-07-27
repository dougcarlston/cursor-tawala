// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Processes
{

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