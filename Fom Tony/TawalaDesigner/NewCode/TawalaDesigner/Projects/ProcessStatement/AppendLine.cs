// $Workfile: AppendLine.cs $
// $Revision: 4 $	$Date: 5/24/06 7:20p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;

namespace Tawala.Projects
{
	/// <summary>
	/// Implements single APPEND line for display in Process window.
	/// </summary>
	[Serializable]
	public class AppendLine : ProcessLine
	{
		public AppendLine()
		{
		}

		public AppendLine(AppendStatement statement) : base (statement, statement.ToString())
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
			AppendStatement appendStatement = statement as AppendStatement;

			Boolean isValid =
				appendStatement.Document != NullObjects.Document &&
                appendStatement.Appendage != NullObjects.Document &&
				appendStatement.ValidateDocumentReference(appendStatement.Document) == ProcessStatement.StatementStatus.Valid &&
				appendStatement.ValidateDocumentReference(appendStatement.Appendage) == ProcessStatement.StatementStatus.Valid;

			return isValid;
		}
	}
}
